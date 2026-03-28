using Microsoft.Extensions.Logging;

namespace UMCore.Tests;


public class TestLogger : ILogger
{
	private static readonly string _path = "../../../../log.txt";
	public Match? Match { get; set; } = null;

	public TestLogger() {
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

		DumpFighterData();
		File.AppendAllText(_path, $"{msg}\n");
	}

	private void DumpFighterData()
	{
		if (Match is null) throw new Exception();

		var str = "-= FIGHTERS START =-\n";
		foreach (var fighter in Match.Fighters)
		{
			str += $"\n{fighter.LogName}: {fighter.Health.Current}\n";
		}
		str += "-= FIGHTERS END =-\n";
		File.AppendAllText(_path, str);
	}

	private class NoopDisposable : IDisposable
	{
		public void Dispose()
		{
		}
	}
}
