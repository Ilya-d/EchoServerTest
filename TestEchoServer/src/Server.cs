using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TestEchoServer {

    /// <summary>
    /// Главный класс сервера, обрабатывает входящие подключения и управляет комнатами
    /// </summary>
	public class Server {
        
        const long ROOM_TIMEOUT = 60 * 1000;

        private bool _isStopRequested;
        private Dictionary<string, Room> _rooms = new Dictionary<string, Room>();
        private ManualResetEvent _allDone = new ManualResetEvent(false);

        private int _portNumber;
        private TcpListener _listener;

		public Server(int portNumber) {
            _portNumber = portNumber;
			_listener = new TcpListener(IPAddress.Any, _portNumber);
			_listener.Start();

			new Thread(ClearRoomsLoop).Start();
			new Thread(ListenRequestsLoop).Start();
		}

        public void Stop() {
            _isStopRequested = true;
        }

        private void ClearRoomsLoop() {
			while (!_isStopRequested) {
                lock(_rooms) {
                    var ids = _rooms.Keys.ToArray();
					foreach (var roomId in ids) {
                        var room = _rooms[roomId];
						if (Utils.CurrentTime() - room.GetLastActivityTime() > ROOM_TIMEOUT) {
                            _rooms.Remove(roomId);
                            room.Clear();
                            Console.WriteLine("Room disposed: " + roomId);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void ListenRequestsLoop() {
            Console.WriteLine("Server started on port " + _portNumber);
            while (!_isStopRequested) {
				// сбрасываем синхронизатор
				_allDone.Reset();

                _listener.BeginAcceptTcpClient(OnUserConnect, _listener);
				// Ждем пока не появится входящее соединение
				_allDone.WaitOne();
            }
            _listener.Stop();
        }

        private void OnUserConnect(IAsyncResult ar) {
			// Сообщаем о подключении пользователя
			_allDone.Set();

			// Get the socket that handles the client request.
            var listener = (TcpListener)ar.AsyncState;
			try {
                var client = listener.EndAcceptTcpClient(ar);
                var user = new User(this, client);
                user.Authorize();
                user.BeginListenForMessages();
			} catch (Exception e) {
                Console.WriteLine(e.ToString());
			}
        }

        public Room GetRoom(string id) {
            lock (_rooms) {
                if (!_rooms.ContainsKey(id)) {
                    _rooms.Add(id, new Room(id));
                    Console.WriteLine("Room created: " + id);
                }
                return _rooms[id];
            }
        }

	}
}
