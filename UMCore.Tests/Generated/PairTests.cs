using System.Text.Json;

namespace UMCore.Tests.Generated;

public class PairTests
{
	private static readonly int NUM_MATCHES = 100;

	[Fact]
	public async Task Medusa_vs_Bigfoot()
	{
		await TestPair(
			"Medusa",
			"Bigfoot"
			// , 20
		);
	}

	[Fact]
	public async Task RobinHood_vs_Alice()
	{
		await TestPair(
			"Robin Hood",
			"Alice"
			// , 20
		);
	}

	// [Fact]
	// public async Task KingArthur_vs_Sinbad()
	// {
	//     await TestPair(
	// 		"King Arthur",
	// 		"Sinbad"
	// 		// , 20
	// 	);
	// }

	private async Task TestPair(string fighter1, string fighter2, int startAt = 0)
	{
		var map = MapTemplate.GetBaskervilleTemplate();
		var core = File.ReadAllText("../../../../core.lua");

		for (int i = startAt; i < startAt + NUM_MATCHES; ++i)
		{
			try
			{

				var seed = i;

				var config = new MatchConfig()
				{
					ActionsPerTurn = 2,
					ExhaustDamage = 1,
					FirstPlayerIdx = 0,
					InitialHandSize = 5,
					ManoeuvreDrawAmount = 1,
					MaxHandSize = 7,
					RandomFirstPlayer = true,
					RandomMatch = false,
					Seed = seed,
					TeamSize = 1,
					TeamCount = 2
				};

				// var logger = new TestLogger();
				var match = new Match(config, map, core)
				{
					Logger = null,
					// Logger = logger,
					// Logger = LoggerFactory.Create(builder => builder
					//         .AddConsole()
					//         .SetMinimumLevel(LogLevel.Debug)
					//     )
					//     .CreateLogger("UMTester")
				};
				// logger.Match = match;

				var first = LoadLoadout($"../../../../.generated/loadouts/{fighter1}/{fighter1}.json");
				var second = LoadLoadout($"../../../../.generated/loadouts/{fighter2}/{fighter2}.json");

				var controller = new RandomPlayerController(seed);

				var players = new QueuedPlayerCollection(config);
				players.AddPlayer("first", 0, first);
				players.AddPlayer("second", 1, second);

				await match.AddPlayers(players, new()
				{
					{"first", controller},
					{"second", controller},
				});

				await match.Run();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed at seed = {i}", e);
			}
		}
	}

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
}