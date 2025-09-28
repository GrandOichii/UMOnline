using Godot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UMClient.v1.Matches;
using UMCore;
using UMCore.Matches;
using UMCore.Matches.Players;
using UMCore.Templates;

public partial class TestMatch : Control
{
	public static IEnumerable<MapNodeLinkTemplate> Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
	{
		return [
			new() {
				First = n1,
				Second = n2,
			},
			new() {
				First = n2,
				Second = n1,
			}
		];
	}

	public static MapTemplate GetBaskervilleTemplate()
	{
		List<MapNodeTemplate> nodes = [
			new() {
				Id = 0,
				Zones = [0],
				HasSecretPassage = true,
			},
			new() {
				Id = 1,
				Zones = [0],
			},
			new() {
				Id = 2,
				Zones = [0],
			},
			new() {
				Id = 3,
				Zones = [0],
			},
			new() {
				Id = 4,
				Zones = [0, 1],
				SpawnNumber = 2,
			},
			new() {
				Id = 5,
				Zones = [0, 6],
			},
			new() {
				Id = 6,
				Zones = [1],
			},
			new() {
				Id = 7,
				Zones = [1],
			},
			new() {
				Id = 8,
				Zones = [1],
			},
			new() {
				Id = 9,
				Zones = [1, 2, 3],
			},
			new() {
				Id = 10,
				Zones = [2],
			},
			new() {
				Id = 11,
				Zones = [2],
			},
			new() {
				Id = 12,
				Zones = [2],
				HasSecretPassage = true,
			},
			new() {
				Id = 13,
				Zones = [3, 4],
			},
			new() {
				Id = 14,
				Zones = [4],
			},
			new() {
				Id = 15,
				Zones = [4],
			},
			new() {
				Id = 16,
				Zones = [4],
				SpawnNumber = 1,
			},
			new() {
				Id = 17,
				Zones = [3],
				HasSecretPassage = true,
			},
			new() {
				Id = 18,
				Zones = [3],
			},
			new() {
				Id = 19,
				Zones = [3, 5],
				SpawnNumber = 3,
			},
			new() {
				Id = 20,
				Zones = [5],
			},
			new() {
				Id = 21,
				Zones = [5],
			},
			new() {
				Id = 22,
				Zones = [4, 5],
			},
			new() {
				Id = 23,
				Zones = [5],
			},
			new() {
				Id = 24,
				Zones = [5],
			},
			new() {
				Id = 25,
				Zones = [5],
			},
			new() {
				Id = 26,
				Zones = [5],
			},
			new() {
				Id = 27,
				Zones = [5],
				HasSecretPassage = true,
			},
			new() {
				Id = 28,
				Zones = [5, 6],
			},
			new() {
				Id = 29,
				Zones = [3, 6],
			},
			new() {
				Id = 30,
				Zones = [6],
			},
			new() {
				Id = 31,
				Zones = [6],
				SpawnNumber = 4,
			},
		];
		return new()
		{
			Nodes = nodes,
			Adjacent = [
				.. Bidirectional(nodes[0], nodes[1]),
				.. Bidirectional(nodes[4], nodes[1]),
				.. Bidirectional(nodes[0], nodes[2]),
				.. Bidirectional(nodes[3], nodes[2]),
				.. Bidirectional(nodes[3], nodes[5]),
				.. Bidirectional(nodes[4], nodes[6]),
				.. Bidirectional(nodes[4], nodes[5]),
				.. Bidirectional(nodes[8], nodes[6]),
				.. Bidirectional(nodes[8], nodes[7]),
				.. Bidirectional(nodes[6], nodes[7]),
				.. Bidirectional(nodes[9], nodes[7]),
				.. Bidirectional(nodes[9], nodes[10]),
				.. Bidirectional(nodes[11], nodes[10]),
				.. Bidirectional(nodes[11], nodes[12]),
				.. Bidirectional(nodes[9], nodes[13]),
				.. Bidirectional(nodes[14], nodes[13]),
				.. Bidirectional(nodes[14], nodes[15]),
				.. Bidirectional(nodes[16], nodes[15]),
				.. Bidirectional(nodes[13], nodes[17]),
				.. Bidirectional(nodes[13], nodes[16]),
				.. Bidirectional(nodes[13], nodes[20]),
				.. Bidirectional(nodes[13], nodes[21]),
				.. Bidirectional(nodes[18], nodes[17]),
				.. Bidirectional(nodes[18], nodes[19]),
				.. Bidirectional(nodes[20], nodes[19]),
				.. Bidirectional(nodes[20], nodes[24]),
				.. Bidirectional(nodes[20], nodes[21]),
				.. Bidirectional(nodes[22], nodes[21]),
				.. Bidirectional(nodes[22], nodes[23]),
				.. Bidirectional(nodes[22], nodes[15]),
				.. Bidirectional(nodes[19], nodes[25]),
				.. Bidirectional(nodes[25], nodes[24]),
				.. Bidirectional(nodes[25], nodes[26]),
				.. Bidirectional(nodes[27], nodes[26]),
				.. Bidirectional(nodes[27], nodes[28]),
				.. Bidirectional(nodes[29], nodes[28]),
				.. Bidirectional(nodes[29], nodes[18]),
				.. Bidirectional(nodes[28], nodes[30]),
				.. Bidirectional(nodes[31], nodes[30]),
				.. Bidirectional(nodes[2], nodes[30]),
				.. Bidirectional(nodes[31], nodes[5]),
			]
		};
	}

	public static MapTemplate GetMapTemplate()
	{
		// o-0-o
		// | | |
		// o o o
		// |   |
		// o-0-o

		//0;0
		var node00 = new MapNodeTemplate()
		{
			Id = 0,
			Zones = [0],
			SpawnNumber = 0,
		};
		//0;1
		var node01 = new MapNodeTemplate()
		{
			Id = 1,
			Zones = [0],
			HasSecretPassage = true,
			SpawnNumber = 2
		};
		//0;2
		var node02 = new MapNodeTemplate()
		{
			Id = 2,
			Zones = [0]
		};
		//1;0
		var node10 = new MapNodeTemplate()
		{
			Id = 10,
			Zones = [0, 1],
			SpawnNumber = 1,
		};
		//1;1
		var node11 = new MapNodeTemplate()
		{
			Id = 11,
			Zones = [0, 1]
		};
		//1;2
		var node12 = new MapNodeTemplate()
		{
			Id = 12,
			Zones = [0, 1],
			SpawnNumber = 3
		};
		//2;0
		var node20 = new MapNodeTemplate()
		{
			Id = 20,
			Zones = [1]
		};
		//2;1
		var node21 = new MapNodeTemplate()
		{
			Id = 21,
			Zones = [1],
			HasSecretPassage = true,
		};
		//2;2
		var node22 = new MapNodeTemplate()
		{
			Id = 22,
			Zones = [1],
		};

		return new()
		{
			Nodes = [node00, node01, node02, node10, node11, node12, node20, node21, node22],
			Adjacent = [
				.. Bidirectional(node00, node01),
				.. Bidirectional(node01, node02),
				.. Bidirectional(node02, node12),
				.. Bidirectional(node12, node22),
				.. Bidirectional(node22, node21),
				.. Bidirectional(node21, node20),
				.. Bidirectional(node20, node10),
				.. Bidirectional(node10, node00),

				.. Bidirectional(node01, node11),
			],
		};
	}

	private static LoadoutTemplate LoadLoadout(string path)
	{
		var data = File.ReadAllText(path);
		var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
		foreach (var card in result.Deck)
		{
			card.Card.Script = File.ReadAllText(card.Card.Script);
		}

		foreach (var fighter in result.Fighters)
		{
			fighter.Script = File.ReadAllText(fighter.Script);
		}
		return result;
	}

	// Called when the node enters the scene tree for the first time.
	public Node ConnectedMatchNode { get; private set; }


	public override void _Ready()
	{
		ConnectedMatchNode = GetNode<Node>("%Match");

		Task.Run(RunMatch);
	}

	private LocalMatchIOHandler _handler;

	public async Task RunMatch()
	{
		try
		{
			var map = GetBaskervilleTemplate();

			var match = new Match(MatchConfig.Default, map, File.ReadAllText("../core.lua"))
			{
				Logger = new GDLogger()
			};

			_handler = new LocalMatchIOHandler(this);
			var controller = new IOPlayerController(_handler);

			var loadout1 = LoadLoadout("../loadouts/Medusa & Harpies.json");
			var loadout2 = LoadLoadout("../loadouts/Dracula & The Sisters.json");

			await match.AddPlayer(
				"RealPlayer",
				0,
				loadout1,
				controller
			);
			await match.AddPlayer(
				"Random",
				1,
				loadout2,
				new DelayedControllerWrapper(TimeSpan.FromMilliseconds(0), new RandomPlayerController(0))
			);

			await match.Run();
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
	}

	public void Load(Godot.Collections.Dictionary data)
	{
		GetNode<Node>("%Connection").EmitSignal("match_info_updated", data);
	}

	public void OnLocalMatchCollectionResponded(string response)
	{
		_handler.SetReadTaskResult(response);
	}
}

public class GDLogger : ILogger
{
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
		GD.Print(msg);
	}

	private class NoopDisposable : IDisposable
	{
		public void Dispose()
		{
		}
	}
}
