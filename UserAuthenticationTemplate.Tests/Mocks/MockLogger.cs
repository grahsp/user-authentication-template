using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UserAuthenticationTemplate.Tests.Mocks
{
    internal class MockLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter != null)
            {
                string logMessage = formatter(state, exception);
                Debug.WriteLine($"[{logLevel}] {logMessage}");
            }
        }
    }
}
