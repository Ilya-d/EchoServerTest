using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TestEchoServer {

    /// <summary>
    /// Класс для обработки подключенного пользователя
    /// </summary>
    public class User {

        private Server _server;
        private TcpClient _client;
        private string _id;
        private string _roomId;
        private Room _room;

        private StreamReader _reader;
        private StreamWriter _writer;

        public User(Server server, TcpClient client) {
            this._server = server;
            this._client = client;
        }

        public void Authorize() {
			_reader = new StreamReader(_client.GetStream());
            _writer = new StreamWriter(_client.GetStream());
			var authorizationString = _reader.ReadLine();
            Utils.ParseAuthorizationString(authorizationString, out _id, out _roomId);
            Console.WriteLine("User authorized: " + _id);
            _room = _server.GetRoom(_roomId);
            _room.AddUser(this);
        }

        public void BeginListenForMessages() {
            new Task(ReadIncomingMessages).Start();
        }

        private void ReadIncomingMessages() {
            while (_reader != null) {
                var message = _reader.ReadLine();
                if (_room != null) {
                    _room.BroadcastMessage(message);
                }
            }
        }

        public void SendMessageAsync(string message) {
            new Task(() => DoSendMessage(message)).Start();
        }

        private void DoSendMessage(string message) {
            if (_writer != null) {
                _writer.WriteLine(message);
                _writer.Flush();
            }
        }

    }
}
