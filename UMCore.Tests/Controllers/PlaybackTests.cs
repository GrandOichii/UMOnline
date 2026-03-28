using Microsoft.Extensions.Logging;

namespace UMCore.Tests.Controllers;

public class PlaybackTests
{
    private readonly static int ITERATIONS = 100;

    [Fact]
    public async Task ShouldPlayback()
    {
        for (int seed = 0; seed < ITERATIONS; ++seed)
        {
            // setup match
            var config = new MatchConfig()
            {
                ActionsPerTurn = MatchConfig.Default1x1.ActionsPerTurn,
                ExhaustDamage = MatchConfig.Default1x1.ExhaustDamage,
                FirstPlayerIdx = MatchConfig.Default1x1.FirstPlayerIdx,
                InitialHandSize = MatchConfig.Default1x1.InitialHandSize,
                ManoeuvreDrawAmount = MatchConfig.Default1x1.ManoeuvreDrawAmount,
                MaxHandSize = MatchConfig.Default1x1.MaxHandSize,
                RandomFirstPlayer = MatchConfig.Default1x1.RandomFirstPlayer,
                RandomMatch = false,
                Seed = seed,
                TeamCount = MatchConfig.Default1x1.TeamCount,
                TeamSize = MatchConfig.Default1x1.TeamSize
            };

            var map = MapTemplate.GetBaskervilleTemplate();
            // var prefix = "..";
            var prefix = "../../../..";
            var core = File.ReadAllText($"{prefix}/core.lua");

            var logger = new RecordTestLogger();
            var match = new Match(config, map, core)
            {
                Logger = logger
            };

            var first = new LoadoutTemplateBuilder("Medusa")
                .Load($"{prefix}/.generated/loadouts/Medusa/Medusa.json")
                .Build();
            var second = new LoadoutTemplateBuilder("Medusa")
                .Load($"{prefix}/.generated/loadouts/Robin Hood/Robin Hood.json")
                .Build();

            var controller1 = new RecorderControllerWrapper(
                new RandomPlayerController(0)
            );
            var controller2 = new RecorderControllerWrapper(
                new RandomPlayerController(1)
            );

            var players = new QueuedPlayerCollection(config);
            players.AddPlayer("first", 0, first);
            players.AddPlayer("second", 1, second);

            await match.AddPlayers(players, new()
            {
                {"first", controller1},
                {"second", controller2},
            });

            await match.Run();

            var playback = new Match(config, map, core)
            {
                Logger = new PlaybackCheckerLogger(logger)
            };

            await playback.AddPlayers(players, new()
            {
                {"first", new ReplayerPlayerController(controller1.Record)},
                {"second", new ReplayerPlayerController(controller2.Record)},
            });

            await playback.Run();
        }
    }

    public class RecordTestLogger : ILogger
    {
        public List<string> Logs { get; }

        public RecordTestLogger() {
            Logs = [];
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
            Logs.Add(msg);
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    public class PlaybackCheckerLogger : ILogger
    {
        private RecordTestLogger _logger;
        private int _idx = 0;
        
        public PlaybackCheckerLogger(RecordTestLogger logger) {
            _logger = logger;
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
            if (msg != _logger.Logs[_idx])
            {
                throw new Exception($"Logs mismatch at idx = {_idx}: Expected: \"{_logger.Logs[_idx]}\", got: \"{msg}\"");
            }
            ++_idx;
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}