using System;
using System.Net.Http;

namespace TestEchoServer {
    
    public static class Utils {

		/// <summary>
		/// Количество тиков в одной милисекунде, у возвращяемого DateTime-ом времени.
		/// </summary>
		public const long TICKS_IN_MILLIS = 10000L;

		/// <summary>
		/// Возвращает текущее время в миллисекундах
		/// </summary>
		/// <returns>The time.</returns>
		public static long CurrentTime() {
            return DateTime.UtcNow.Ticks / TICKS_IN_MILLIS;
		}

		/// <summary>
		/// Разделяет особую строку на id пользователя и комнаты
		/// </summary>
		/// <param name="s">S.</param>
		/// <param name="userId">User identifier.</param>
		/// <param name="roomId">Room identifier.</param>
		public static void ParseAuthorizationString(string s, out string userId, out string roomId) {
			var split = s.Split(':');
            userId = split[0];
            roomId = split[1];
        }

		/// <summary>
		/// Собирает id пользователя и комнаты в одну строку для пересылки
		/// </summary>
		/// <returns>The authorization string.</returns>
		/// <param name="userId">User identifier.</param>
		/// <param name="roomId">Room identifier.</param>
		public static string CreateAuthorizationString(string userId, string roomId) {
            return userId + ':' + roomId;
        }

	}
}
