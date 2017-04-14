using System;
using System.Collections.Generic;

namespace TestEchoServer {

    /// <summary>
    /// Комната, хранит список своих пользователей, позволяет делать рассылки.
    /// </summary>
	public class Room {

        private string _id;
		private List<User> _users = new List<User>();
        private long _lastActivityTime;

		public Room(string id) {
            this._id = id;
            _lastActivityTime = DateTime.UtcNow.Ticks;
		}

        public void BroadcastMessage(string message) {
            //Console.WriteLine("Broadcast: " + message);
            _lastActivityTime = Utils.CurrentTime();
            lock (_users) {
                foreach (var user in _users) {
                    user.SendMessageAsync(message);
                }
            }
        }

        public long GetLastActivityTime() {
            return _lastActivityTime;
        }

        public void Clear() {
            lock (_users) {
                _users.Clear();
            }
        }

        public void AddUser(User user) {
            lock (_users) {
                if (!_users.Contains(user)) {
                    _users.Add(user);
                }
            }
        }

		public void RemoveUser(User user) {
			lock (_users) {
				_users.Remove(user);
			}
		}
   }
}
