using Microsoft.Extensions.Logging;
using ProvisioningPipeline.Interfaces;

namespace TestApp {
	public class ConsoleLogger : ILogger {
		private void LogInformation(string message) {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[INFO] {message}");
			Console.ResetColor();
		}

		private void LogError(Exception exception, string message) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"[ERROR] {message} - Exception: {exception.Message}");
			Console.ResetColor();
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
			switch (logLevel) {
				case LogLevel.Information:
					LogInformation(formatter(state, exception));
					break;
				case LogLevel.Error:
					LogError(exception ?? new Exception("No exception provided"), formatter(state, exception));
					break;
				default:
					Console.WriteLine($"[{logLevel}] {formatter(state, exception)}");
					break;
			}
		}

		public bool IsEnabled(LogLevel logLevel) {
			throw new NotImplementedException();
		}

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
			throw new NotImplementedException();
		}
	}
}