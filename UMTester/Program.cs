using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Templates;

using CommandLine;
using UMCore.Matches.Players.Controllers;

public class ConsolePlayerController : IPlayerController
{
    private void PrintInfo(Fighter fighter)
    {
        System.Console.WriteLine($"\t{fighter.LogName}: {fighter.Health.Current}/{fighter.Health.Max}");
    }

    private void PrintInfo(Player player)
    {
        foreach (var node in player.Match.Map.Nodes)
            System.Console.WriteLine($"{node.Id} -> {node.Fighter?.LogName}");
        foreach (var fighter in player.Fighters)
            PrintInfo(fighter);

        System.Console.WriteLine($"-= {player.LogName} =-");
        System.Console.WriteLine($"Hand count: {player.Hand.Count}");
        System.Console.WriteLine($"Deck count: {player.Deck.Count}");
        System.Console.WriteLine($"Discard count: {player.DiscardPile.Count}");
        System.Console.WriteLine($"Actions left: {player.ActionCount}");
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        PrintInfo(player);
        System.Console.WriteLine($"Choose action: {string.Join(", ", options)}");
        var nodes = options.ToList();
        for (int i = 0; i < nodes.Count; ++i)
            System.Console.WriteLine($"{i}: {nodes[i]}");
        var result = Console.ReadLine()!;
        return nodes[int.Parse(result)];
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var nodes = options.ToList();
        for (int i = 0; i < nodes.Count; ++i)
            System.Console.WriteLine($"{i}: {nodes[i].Id}");
        var result = Console.ReadLine()!;
        return nodes[int.Parse(result)];
    }

    public async Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var cards = options.ToList();
        for (int i = 0; i < cards.Count; ++i)
            System.Console.WriteLine($"{i}: {cards[i].LogName}");
        var result = Console.ReadLine()!;
        return cards[int.Parse(result)];
    }

    public async Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var cards = options.ToList();
        for (int i = 0; i < cards.Count; ++i)
            System.Console.WriteLine($"{i}: {cards[i].LogName}");
        var result = Console.ReadLine()!;
        if (result == "") return null;
        return cards[int.Parse(result)];
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var fighters = options.ToList();
        for (int i = 0; i < fighters.Count; ++i)
            System.Console.WriteLine($"{i}: {fighters[i].LogName}");
        var result = Console.ReadLine()!;
        return fighters[int.Parse(result)];
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        PrintInfo(player);
        System.Console.WriteLine("Choose how to attack");
        var attacks = options.ToList();
        for (int i = 0; i < attacks.Count; ++i)
            System.Console.WriteLine($"{i}: {attacks[i].Fighter.LogName} -> {attacks[i].Target.LogName} [{attacks[i].AttackCard.LogName}]");
        var result = Console.ReadLine()!;
        return attacks[int.Parse(result)];
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var opts = options.ToList();
        for (int i = 0; i < opts.Count; ++i)
            System.Console.WriteLine($"{i}: {opts[i]}");
        var result = Console.ReadLine()!;
        return opts[int.Parse(result)];
    }

    public async Task Update(Player player)
    {
    }

    public async Task Setup(Player player, Match.SetupData setupData)
    {
    }

    public void AddEvent(Event e)
    {
    }

    public void AddLog(Log l)
    {
    }

    public Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<UMCore.Matches.Path> ChoosePath(Player player, UMCore.Matches.Path[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        throw new NotImplementedException();
    }
}

// public class AppArgs
// {
//     [Option("first", Required = true, HelpText = "Path to first fighter")]
//     public required string FirstFighterPath { get; set; }
//     [Option("second", Required = true, HelpText = "Path to second fighter")]
//     public required string SecondFighterPath { get; set; }
//     [Option("times", Required = false, Default = 1, HelpText = "Amount of times to run the match")]
//     public required int Times { get; set; }
//     [Option("log", Required = false, Default = false, HelpText = "Print match logs")]
//     public required bool Log { get; set; }
//     [Option("seed", Required = false, Default = null, HelpText = "First match seed")]
//     public required int? FirstSeed { get; set; }
// }

// public struct Report
// {
//     public int FinishedCount { get; set; }
//     public int CrashedCount { get; set; }

//     public Report()
//     {
//         FinishedCount = 0;
//         CrashedCount = 0;
//     }

//     public void Print()
//     {
//         System.Console.WriteLine($"Finished successfully: {FinishedCount}");
//         System.Console.WriteLine($"Crashed: {CrashedCount}");
//     }

//     public void ProcessFinished(Match match)
//     {
//         FinishedCount += 1;
//     }

//     public void ProcessCrashed(Match match)
//     {
//         CrashedCount += 1;
//     }
// }

public class Program
{
    // public static MapNodeLinkTemplate[] Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
    // {
    //     return [
    //         new() {
    //             First = n1.Id,
    //             Second = n2.Id,
    //         },
    //     ];
    // }

    // public static MapTemplate GetMapTemplate()
    // {
    //     // o-0-o
    //     // | | |
    //     // o o o
    //     // |   |
    //     // o-0-o

    //     //0;0
    //     var node00 = new MapNodeTemplate()
    //     {
    //         Id = 0,
    //         Zones = [0],
    //         SpawnNumber = 0,
    //     };
    //     //0;1
    //     var node01 = new MapNodeTemplate()
    //     {
    //         Id = 1,
    //         Zones = [0],
    //         SpawnNumber = 2
    //     };
    //     //0;2
    //     var node02 = new MapNodeTemplate()
    //     {
    //         Id = 2,
    //         Zones = [0]
    //     };
    //     //1;0
    //     var node10 = new MapNodeTemplate()
    //     {
    //         Id = 10,
    //         Zones = [0, 1],
    //         SpawnNumber = 1,
    //     };
    //     //1;1
    //     var node11 = new MapNodeTemplate()
    //     {
    //         Id = 11,
    //         Zones = [0, 1]
    //     };
    //     //1;2
    //     var node12 = new MapNodeTemplate()
    //     {
    //         Id = 12,
    //         Zones = [0, 1],
    //         SpawnNumber = 3
    //     };
    //     //2;0
    //     var node20 = new MapNodeTemplate()
    //     {
    //         Id = 20,
    //         Zones = [1]
    //     };
    //     //2;1
    //     var node21 = new MapNodeTemplate()
    //     {
    //         Id = 21,
    //         Zones = [1],
    //     };
    //     //2;2
    //     var node22 = new MapNodeTemplate()
    //     {
    //         Id = 22,
    //         Zones = [1],
    //     };

    //     return new()
    //     {
    //         Nodes = [node00, node01, node02, node10, node11, node12, node20, node21, node22],
    //         Adjacent = [
    //             .. Bidirectional(node00, node01),
    //             .. Bidirectional(node01, node02),
    //             .. Bidirectional(node02, node12),
    //             .. Bidirectional(node12, node22),
    //             .. Bidirectional(node22, node21),
    //             .. Bidirectional(node21, node20),
    //             .. Bidirectional(node20, node10),
    //             .. Bidirectional(node10, node00),

    //             .. Bidirectional(node01, node11),
    //         ]
    //     };
    // }

    private static LoadoutTemplate LoadLoadout(string path)
    {
        var data = File.ReadAllText(path);
        var loadoutPath = System.IO.Path.GetDirectoryName(path);
        var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
        foreach (var card in result.Deck)
        {
            card.Script = File.ReadAllText(System.IO.Path.Join(loadoutPath, card.Script));
        }

        foreach (var fighter in result.Fighters)
        {
            fighter.Script = File.ReadAllText(System.IO.Path.Join(loadoutPath, fighter.Script));
        }
        return result;
    }

    public static async Task Main(string[] args)
    {
        // var appArgs = Parser.Default.ParseArguments<AppArgs>(args).Value;
        // if (appArgs is null) return;

        // var report = new Report();
        // var map = GetMapTemplate();
        // var core = File.ReadAllText("../core.lua");
        // // ILogger? logger = null;
        // ILogger? logger = appArgs.Log
        //         ? LoggerFactory.Create(builder => builder
        //                 .AddConsole()
        //                 .SetMinimumLevel(LogLevel.Debug)
        //             )
        //             .CreateLogger("UMTester")
        //         : null;

        // var rnd = new Random();
                
        // for (int i = 0; i < appArgs.Times; ++i)
        // {
        //     var seed = rnd.Next();
        //     if (i == 0 && appArgs.FirstSeed != null)
        //         seed = (int)appArgs.FirstSeed;

        //     var config = new MatchConfig()
        //     {
        //         ActionsPerTurn = 2,
        //         ExhaustDamage = 1,
        //         FirstPlayerIdx = 0,
        //         InitialHandSize = 5,
        //         ManoeuvreDrawAmount = 1,
        //         MaxHandSize = 7,
        //         RandomFirstPlayer = true,
        //         RandomMatch = false,
        //         Seed = seed,
        //         TeamSize = 1,
        //         TeamCount = 2, 
        //     };

        //     var match = new Match(config, map, core)
        //     {
        //         Logger = logger
        //     };

        //     var first = LoadLoadout(appArgs.FirstFighterPath);
        //     var second = LoadLoadout(appArgs.SecondFighterPath);

        //     var controller = new RandomPlayerController(seed);
        //     var players = new QueuedPlayerCollection(config);
        //     players.AddPlayer("first", 0, first);
        //     players.AddPlayer("second", 1, second);

        //     await match.AddPlayers(players, new()
        //     {
        //         {"first", controller},
        //         {"second", controller},
        //     });

        //     try
        //     {
        //         await match.Run();
        //         report.ProcessFinished(match);
        //     }
        //     catch (Exception e)
        //     {
        //         if (appArgs.Log)
        //         {
        //             await Task.Delay(1000);
        //             System.Console.WriteLine(e);
        //             System.Console.WriteLine(e.StackTrace);
        //             System.Console.WriteLine("-============-");
        //             System.Console.WriteLine(e.InnerException);
        //             System.Console.WriteLine(e.InnerException?.StackTrace);
        //         }
        //         report.ProcessCrashed(match);
        //     }
        // }

        // await Task.Delay(1000);

        // report.Print();

        // return;

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
            Seed = 0,
            TeamCount = MatchConfig.Default1x1.TeamCount,
            TeamSize = MatchConfig.Default1x1.TeamSize
        };

        var map = MapTemplate.GetBaskervilleTemplate();
        var prefix = "..";
        // var prefix = "../../../..";
        var core = File.ReadAllText($"{prefix}/core.lua");

        var logger = new RecordTestLogger();
        var match = new Match(config, map, core)
        {
            Logger = logger
        };

        var first = LoadLoadout($"{prefix}/.generated/loadouts/Medusa/Medusa.json");
        var second = LoadLoadout($"{prefix}/.generated/loadouts/Robin Hood/Robin Hood.json");

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

        System.Console.WriteLine("Starting initial match...");

        await match.Run();

        System.Console.WriteLine("Match finished!");
        System.Console.WriteLine("Setting up playback match");

        var playback = new Match(config, map, core)
        {
            Logger = new PlaybackCheckerLogger(logger)
        };

        await playback.AddPlayers(players, new()
        {
            {"first", new ReplayerPlayerController(controller1.Record)},
            {"second", new ReplayerPlayerController(controller2.Record)},
        });

        System.Console.WriteLine("Running playback match...");
        await playback.Run();
        System.Console.WriteLine("Playback finished!");

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

