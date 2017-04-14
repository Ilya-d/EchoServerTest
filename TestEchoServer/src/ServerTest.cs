using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace TestEchoServer {
    
    public class ServerTest {

        public static void Main(string[] args) {
            var portNumber = 7777;
            var server = new Server(portNumber);

            var serverAddress = "localhost";
            for (int i = 0; i < 5; i++) {
                new Bot(serverAddress, portNumber, "bot" + i, "room" + (i/3));
            }
        }
    }
}
