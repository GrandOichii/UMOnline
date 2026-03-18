using Godot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using UMCore;
using UMCore.Matches;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Controllers;
using UMCore.Templates;

public class TestMatchIOHandler(TestMatch match) : IIOHandler
{
	private TaskCompletionSource<string> _readTask = null;
	public void SetReadTaskResult(string result)
	{
		_readTask.SetResult(result);
	}

	public Task Close()
	{
		// TODO
		return Task.CompletedTask;
	}

	public Task<string> Read()
	{
		_readTask = new();

		return _readTask.Task;
	}

	public async Task Write(UpdateInfo info)
	{
		match.CallDeferred("Load", Json.ParseString(JsonSerializer.Serialize(info)));
	}
}


public partial class TestMatch : Control
{
	[Export(PropertyHint.Enum, "Medusa,Ms. Marvel,Daredevil,Sinbad,Sherlock Holmes,Buffy,Hamlet,Black Widow,Angel,Spike,Alice,Dr. Ellie Sattler,Beowulf,Robin Hood,Dracula,Bigfoot,Achilles,Jekyll & Hyde,Titania,Rosie the Riveter,Little Red Riding Hood,Willow,Luke Cage,Bloody Mary,Sun Wukong,Black Panther,The Wayward Sisters,Invisible Man,InGen,Yennenga,Bullseye,Moon Knight,Raptors,Harry Houdini,Squirrel Girl,Ghost Rider,Muhammad Ali,Bruce Lee,Ciri,Ancient Leshen,Eredin,Philippa,Leonardo,Raphael,Elektra,T. Rex,Cloak and Dagger,The Genie,Winter Soldier,Nikola Tesla,William Shakespeare,Dr. Jill Trent,Golden Bat,Annie Christmas,Spider-Man,She-Hulk,Doctor Strange,Tomoe Gozen,Oda Nobunaga,Geralt of Rivia,Yennefer & Triss,Triss & Yennefer,King Arthur,Shredder,Krang,Donatello,Michelangelo,Chupacabra,Loki,Pandora,Blackbeard,Muhammad Ali")]
	public string PlayerDeck { get; private set; }
	[Export(PropertyHint.Enum, "Medusa,Ms. Marvel,Daredevil,Sinbad,Sherlock Holmes,Buffy,Hamlet,Black Widow,Angel,Spike,Alice,Dr. Ellie Sattler,Beowulf,Robin Hood,Dracula,Bigfoot,Achilles,Jekyll & Hyde,Titania,Rosie the Riveter,Little Red Riding Hood,Willow,Luke Cage,Bloody Mary,Sun Wukong,Black Panther,The Wayward Sisters,Invisible Man,InGen,Yennenga,Bullseye,Moon Knight,Raptors,Harry Houdini,Squirrel Girl,Ghost Rider,Muhammad Ali,Bruce Lee,Ciri,Ancient Leshen,Eredin,Philippa,Leonardo,Raphael,Elektra,T. Rex,Cloak and Dagger,The Genie,Winter Soldier,Nikola Tesla,William Shakespeare,Dr. Jill Trent,Golden Bat,Annie Christmas,Spider-Man,She-Hulk,Doctor Strange,Tomoe Gozen,Oda Nobunaga,Geralt of Rivia,Yennefer & Triss,Triss & Yennefer,King Arthur,Shredder,Krang,Donatello,Michelangelo,Chupacabra,Loki,Pandora,Blackbeard,Muhammad Ali")]
	public string BotDeck { get; private set; }
	private static LoadoutTemplate LoadLoadout(string path)
	{
		var data = File.ReadAllText(path);
		var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
		var dir = System.IO.Path.GetDirectoryName(path);
		foreach (var card in result.Deck)
		{
			card.Script = File.ReadAllText($"{dir}/{card.Script}");
		}

		foreach (var fighter in result.Fighters)
		{
			fighter.Script = File.ReadAllText($"{dir}/{fighter.Script}");
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

	private TestMatchIOHandler _handler;

	public async Task RunMatch()
	{
		try
		{
			var map = MapTemplate.GetBaskervilleTemplate();

			var config = new MatchConfig()
			{
				RandomMatch = false,
				Seed = 0,
				InitialHandSize = MatchConfig.Default1x1.InitialHandSize,
				ActionsPerTurn = MatchConfig.Default1x1.ActionsPerTurn,
				MaxHandSize = MatchConfig.Default1x1.MaxHandSize,
				ManoeuvreDrawAmount = MatchConfig.Default1x1.ManoeuvreDrawAmount,
				RandomFirstPlayer = MatchConfig.Default1x1.RandomFirstPlayer,
				FirstPlayerIdx = MatchConfig.Default1x1.FirstPlayerIdx,
				ExhaustDamage = MatchConfig.Default1x1.ExhaustDamage,
				TeamSize = MatchConfig.Default1x1.TeamSize,
				TeamCount = MatchConfig.Default1x1.TeamCount,
			};

			var match = new Match(config, map, File.ReadAllText("../core.lua"))
			{
				Logger = new GDLogger()
			};

			_handler = new TestMatchIOHandler(this);
			var controller = new IOPlayerController(_handler);

			var loadout1 = LoadLoadout($"../.generated/loadouts/{PlayerDeck}/{PlayerDeck}.json");
			var loadout2 = LoadLoadout($"../.generated/loadouts/{BotDeck}/{BotDeck}.json");

			var opponentController = new DelayedControllerWrapper(
				new RandomPlayerController(0),
				TimeSpan.FromSeconds(1)
			);
			// var opponentController = new LuaPlayerController(
			// 	File.ReadAllText("../bots/smart.lua")
			// );
			var players = new QueuedPlayerCollection(config);
            players.AddPlayer("RealPlayer", 0, loadout1);
            players.AddPlayer("Random", 1, loadout2);

            await match.AddPlayers(players, new()
            {
                {"first", controller},
                {"second", opponentController},
            });

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
