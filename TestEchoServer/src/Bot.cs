using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TestEchoServer {

    /// <summary>
    /// Бот, который подключается к указанному серверу, авторизовыывается, отправляет и читает сообщения
    /// </summary>
    public class Bot {

        private string _serverAddress;
        private int _portNumber;
        private string _userId;
        private string _roomId;

        private StreamWriter _writer;
        private StreamReader _reader;

        public Bot(string serverAddress, int portNumber, string userId, string roomId) {
            this._serverAddress = serverAddress;
            this._portNumber = portNumber;
            this._userId = userId;
            this._roomId = roomId;
            new Task(RunBotLogic).Start();
            //Console.WriteLine("Bot " + _userId + " is created");
        }
       
        private void RunBotLogic() {
            var client = new TcpClient(_serverAddress, _portNumber);
            var stream = client.GetStream();
            _writer = new StreamWriter(stream);
            _reader = new StreamReader(stream);

            // Запускаем считывание входящих сообщений
            new Task(ReadIncomingMessagesLoop).Start();

            // Авторизация
            var authorizationString = Utils.CreateAuthorizationString(_userId, _roomId);
            _writer.WriteLine(authorizationString);
            _writer.Flush();

            // Отправка сообщений
            int messageIndex = 0;
            while (messageIndex < 100) {
                _writer.WriteLine("test" + messageIndex + " from " + _userId);
                _writer.Flush();
                //Console.WriteLine("Send message" + messageIndex);
                messageIndex++;
                Thread.Sleep(100);
            }
        }

        private void ReadIncomingMessagesLoop() {
            while(true) {
                var message = _reader.ReadLine();
                Console.WriteLine("User " + _userId + " received message: " + message);
            }
        }

    }
}
