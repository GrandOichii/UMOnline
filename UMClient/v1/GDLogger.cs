
using System;
using System.IO;
using Microsoft.Extensions.Logging;

public class GDLogger : ILogger
{
	private static readonly string _path = "log.txt";
	public GDLogger() {
		File.WriteAllText(_path, "");
	}

	public IDisposable BeginScope<TState>(TState state) where TState : notnull
	{
		return new NoopDisposable();
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		var msg = formatter(state, exception);
		File.AppendAllText(_path, $"{msg}\n");
	}

	private class NoopDisposable : IDisposable
	{
		public void Dispose()
		{
		}
	}
}
