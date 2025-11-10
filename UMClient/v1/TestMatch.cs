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
	[Export(PropertyHint.Enum, "Medusa,Ms. Marvel,Daredevil,Sinbad,Sherlock Holmes,Buffy,Hamlet,Black Widow,Angel,Spike,Alice,Dr. Ellie Sattler,Beowulf,Robin Hood,Dracula,Bigfoot,Achilles,Jekyll & Hyde,Titania,Rosie the Riveter,Little Red Riding Hood,Willow,Luke Cage,Bloody Mary,Sun Wukong,Black Panther,The Wayward Sisters,Invisible Man,InGen,Yennenga,Bullseye,Moon Knight,Raptors,Harry Houdini,Squirrel Girl,Ghost Rider,Muhammad Ali,Bruce Lee,Ciri,Ancient Leshen,Eredin,Philippa,Leonardo,Raphael,Elektra,T. Rex,Cloak and Dagger,The Genie,Winter Soldier,Nikola Tesla,William Shakespeare,Dr. Jill Trent,Golden Bat,Annie Christmas,Spider-Man,She-Hulk,Doctor Strange,Tomoe Gozen,Oda Nobunaga,Geralt of Rivia,Yennefer & Triss,Triss & Yennefer,King Arthur,Shredder,Krang,Donatello,Michelangelo,Chupacabra,Loki,Pandora,Blackbeard")]
	public string PlayerDeck { get; private set; } 
	[Export(PropertyHint.Enum, "Medusa,Ms. Marvel,Daredevil,Sinbad,Sherlock Holmes,Buffy,Hamlet,Black Widow,Angel,Spike,Alice,Dr. Ellie Sattler,Beowulf,Robin Hood,Dracula,Bigfoot,Achilles,Jekyll & Hyde,Titania,Rosie the Riveter,Little Red Riding Hood,Willow,Luke Cage,Bloody Mary,Sun Wukong,Black Panther,The Wayward Sisters,Invisible Man,InGen,Yennenga,Bullseye,Moon Knight,Raptors,Harry Houdini,Squirrel Girl,Ghost Rider,Muhammad Ali,Bruce Lee,Ciri,Ancient Leshen,Eredin,Philippa,Leonardo,Raphael,Elektra,T. Rex,Cloak and Dagger,The Genie,Winter Soldier,Nikola Tesla,William Shakespeare,Dr. Jill Trent,Golden Bat,Annie Christmas,Spider-Man,She-Hulk,Doctor Strange,Tomoe Gozen,Oda Nobunaga,Geralt of Rivia,Yennefer & Triss,Triss & Yennefer,King Arthur,Shredder,Krang,Donatello,Michelangelo,Chupacabra,Loki,Pandora,Blackbeard")]
	public string BotDeck { get; private set; } 

	public static IEnumerable<MapNodeLinkTemplate> Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
	{
		return [
			new() {
				First = n1.Id,
				Second = n2.Id,
			},
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
				.. Bidirectional(nodes[0], nodes[2]),
				.. Bidirectional(nodes[0], nodes[1]),
				.. Bidirectional(nodes[4], nodes[1]),
				.. Bidirectional(nodes[4], nodes[5]),
				.. Bidirectional(nodes[4], nodes[6]),
				.. Bidirectional(nodes[7], nodes[6]),
				.. Bidirectional(nodes[7], nodes[8]),
				.. Bidirectional(nodes[7], nodes[9]),
				.. Bidirectional(nodes[8], nodes[9]),
				.. Bidirectional(nodes[10], nodes[9]),
				.. Bidirectional(nodes[11], nodes[9]),
				.. Bidirectional(nodes[13], nodes[9]),
				.. Bidirectional(nodes[13], nodes[14]),
				.. Bidirectional(nodes[15], nodes[14]),
				.. Bidirectional(nodes[15], nodes[16]),
				.. Bidirectional(nodes[13], nodes[16]),
				.. Bidirectional(nodes[13], nodes[17]),
				.. Bidirectional(nodes[13], nodes[21]),
				.. Bidirectional(nodes[22], nodes[21]),
				.. Bidirectional(nodes[22], nodes[23]),
				.. Bidirectional(nodes[22], nodes[15]),
				.. Bidirectional(nodes[21], nodes[23]),
				.. Bidirectional(nodes[18], nodes[17]),
				.. Bidirectional(nodes[11], nodes[10]),
				.. Bidirectional(nodes[12], nodes[10]),
				.. Bidirectional(nodes[13], nodes[20]),
				.. Bidirectional(nodes[21], nodes[20]),
				.. Bidirectional(nodes[19], nodes[20]),
				.. Bidirectional(nodes[19], nodes[24]),
				.. Bidirectional(nodes[19], nodes[18]),
				.. Bidirectional(nodes[29], nodes[18]),
				.. Bidirectional(nodes[29], nodes[28]),
				.. Bidirectional(nodes[27], nodes[28]),
				.. Bidirectional(nodes[27], nodes[26]),
				.. Bidirectional(nodes[25], nodes[26]),
				.. Bidirectional(nodes[25], nodes[19]),
				.. Bidirectional(nodes[25], nodes[24]),
				.. Bidirectional(nodes[28], nodes[30]),
				.. Bidirectional(nodes[31], nodes[30]),
				.. Bidirectional(nodes[2], nodes[30]),
				.. Bidirectional(nodes[31], nodes[5]),
				.. Bidirectional(nodes[24], nodes[20]),
				.. Bidirectional(nodes[6], nodes[8]),
				.. Bidirectional(nodes[3], nodes[5]),
				.. Bidirectional(nodes[3], nodes[2]),
			]
		};
	}

	private static LoadoutTemplate LoadLoadout(string path)
	{
		var data = File.ReadAllText(path);
		var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
		foreach (var card in result.Deck)
		{
			card.Card.Script = File.ReadAllText($"../{card.Card.Script}");
		}

		foreach (var fighter in result.Fighters)
		{
			fighter.Script = File.ReadAllText($"../{fighter.Script}");
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

			var match = new Match(MatchConfig.Testing, map, File.ReadAllText("../core.lua"))
			{
				Logger = new GDLogger()
			};

			_handler = new LocalMatchIOHandler(this);
			var controller = new IOPlayerController(_handler);

			var loadout1 = LoadLoadout($"../.generated/loadouts/{PlayerDeck}/{PlayerDeck}.json");
			var loadout2 = LoadLoadout($"../.generated/loadouts/{BotDeck}/{BotDeck}.json");

			await match.AddPlayer(
				"RealPlayer",
				0,
				loadout1,
				controller
				// new DelayedControllerWrapper(TimeSpan.FromMilliseconds(10), new RandomPlayerController(0))
			);
			await match.AddPlayer(
				"Random",
				1,
				loadout2,
				new DelayedControllerWrapper(TimeSpan.FromMilliseconds(10), new RandomPlayerController(0))
			);

			await match.Run();
		}
		catch (Exception e)
		{
			GD.PushError(e);
			GD.Print(e.Message);
			GD.Print(e.StackTrace);
			GD.Print("");
			GD.Print("");
			GD.Print("---====================----");
			GD.Print("");
			GD.Print("");
			GD.Print(e.InnerException?.Message);
			GD.Print(e.InnerException?.StackTrace);
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
