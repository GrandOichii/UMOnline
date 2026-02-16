using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Microsoft.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;
using UMCore.Matches.Players.Cards;
using UMCore.Templates;
using UMDTO;

public partial class LocalRepository : Node
{
	#region Signals

	[Signal]
	public delegate void ContentUpdateProcessedEventHandler();

	#endregion

	private static readonly SqliteCompiler SQL_COMPILER = new();

	#region Exports

	[Export]
	public string DataSource { get; set; }
	[Export]
	public bool DropOnLaunch { get; set; }
	[Export]
	public string CardBackDirectoryName { get; set; }
	[Export]
	public string FighterImageDirectoryName { get; set; }
	[Export]
	public string CardImageDirectoryName { get; set; }

	#endregion

	private readonly List<IModel> _defaultModels = [
		CoreModel.Default,
		DeckModel.Default,
		ScriptModel.Default,
		CardModel.Default,
		FighterModel.Default,
		AppStateModel.Default,
	];

	private SqliteConnection _connection;

	private string CardBackDirectory() => $"user://{CardBackDirectoryName}";
	private string GetFighterImageDirectory() => $"user://{FighterImageDirectoryName}";
	private string GetCardImageDirectory() => $"user://{CardImageDirectoryName}";

	public override void _Ready()
	{
		var path = ProjectSettings.GlobalizePath(DataSource);
		_connection = new SqliteConnection($"Data Source={path}");
		_connection.Open();

		// drop tables
		if (DropOnLaunch)
		{
			var reversed = new List<IModel>(_defaultModels);
			reversed.Reverse();
			foreach (var model in reversed)
			{
				GD.Print($"DROP {model.SQLTableName()}");
				ExecNonQuery(model.SQLDrop());
			}
		}

		// create tables
		foreach (var model in _defaultModels)
		{
			ExecNonQuery(model.SQLCreate());
		}

		// create app state
		var existing = GetAppState();
		if (existing is null)
		{
			InsertModel(AppStateModel.Default);
		}

		// TODO remove
		// if (DropOnLaunch)
		// 	InsertDummyData();

		// create user data directories
		var err = DirAccess.MakeDirRecursiveAbsolute(CardBackDirectory());
		// TODO handle err
		err = DirAccess.MakeDirRecursiveAbsolute(GetFighterImageDirectory());
		// TODO handle err
		err = DirAccess.MakeDirRecursiveAbsolute(GetCardImageDirectory());
		// TODO handle err
	}

	private void ExecNonQuery(string command)
	{
		var comm = new SqliteCommand(command, _connection);
		comm.ExecuteNonQuery();
	}

	private void InsertDummyData()
	{
		List<DeckModel> decks = [
			new DeckModel()
			{
				ChoosesSidekick = false,
				Editable = true,
				Id = -1,
				MaxHandSize = 10,
				Name = "editable1",
				StartingHandSize = 23,
				StartsWithSidekicks = false,
				Description = "This is an editable deck",
				CardBackPath = null,
			},
			new DeckModel()
			{
				ChoosesSidekick = true,
				Editable = false,
				Id = -1,
				MaxHandSize = 1,
				Name = "non-editable2",
				StartingHandSize = 1,
				StartsWithSidekicks = true,
				Description = "This is a non-editable deck",
				CardBackPath = null,
			}
		];

		foreach (var deck in decks)
		{
			InsertDeck(deck);
		}

		var xScale = 400;
		var yScale = 300;
        EditorState editor(int x, int y) => new() { X = x * xScale, Y = y * yScale };

        var graphState1 = new ScriptState()
		{
			Connections = [
				new() {
					From = 0,
					FromSlot = 3,
					To = 1,
					ToSlot = 0,
				},
				new() {
					From = 1,
					FromSlot = 0,
					To = 2,
					ToSlot = 0,
				},
				new() {
					From = 4,
					FromSlot = 0,
					To = 3,
					ToSlot = 0,
				},
				new() {
					From = 3,
					FromSlot = 0,
					To = 2,
					ToSlot = 1,
				},
				new() {
					From = 5,
					FromSlot = 0,
					To = 2,
					ToSlot = 2,
				},
			],
			Nodes = [
				new() {
					Id = 2,
					Name = "DiscardEffect",
					Editor = editor(2, 1),
					Data = new() {
						{ "random", false }
					}
				},
				new() {
					Id = 4,
					Name = "OnlyPlayerFilter",
					Editor = editor(0, 1),
					Data = new() {
						{ "player", 1 }
					}
				},
				new() {
					Id = 5,
					Name = "ConstNumeric",
					Editor = editor(1, 2),
					Data = new() {
						{ "number", 1 }
					}
				},
				new() {
					Id = 1,
					Name = "ability",
					Editor = editor(1, 0),
					Data = new() {
						{ "text", "After combat: your opponent discards 1 card." }
					},
				},
				new() {
					Id = 3,
					Name = "players",
					Editor = editor(1, 1),
					Data = new() {
						{ "single", true },
						{ "outputCount", 1 },
					},
				},
				new()
				{
					Id = 0,
					Name = "CardStart",
					Editor = editor(0, 0),
					Data = [],
				}
			],
		};

		{
			var deck = GetDeck("non-editable2");

			var fighter = new FighterModel()
			{
				Amount = 3,
				CanMoveOverOpposing = false,
				DeckId = deck.Id,
				Id = -1,
				ImagePath = null,
				IsRanged = false,
				IsSidekick = false,
				IsSmall = false,
				MaxHealth = 13,
				MeleeRange = 1,
				Movement = 3,
				Name = "fighter1",
				StartingHealth = 13,
				Text = "fighter1 text here",
				ScriptId = -1,
			};
			InsertFighter(fighter);

			var card = new CardModel()
			{
				Id = -1,
				DeckId = deck.Id,
				ScriptId = -1,
				AllowedFighters = "Fighter1,Fighter2",
				Labels = "Label1,label2",
				Boost = -1,
				Count = 2,
				ImagePath = null,
				Name = "c1",
				StartingHandCount = 0,
				Text = "c1 text here",
				Title = "Card1",
				Type = CardModelType.Scheme,
				Value = 0,
			};
			InsertCard(card);

			var cardId = (int)(long)LastInsertedId(card.SQLTableName());
			var c = GetCard(cardId);
			var script = GetScriptModel(c.ScriptId);
			script.GraphState = graphState1.ToJson();
			UpdateScriptById(script);
		}
		{
			var deck = GetDeck("editable1");

			var fighter1 = new FighterModel()
			{
				Amount = 1,
				CanMoveOverOpposing = true,
				DeckId = deck.Id,
				Id = -1,
				ImagePath = null,
				IsRanged = true,
				IsSidekick = false,
				IsSmall = false,
				MaxHealth = 9,
				MeleeRange = 1,
				Movement = 2,
				Name = "fighter2",
				StartingHealth = 9,
				Text = "fighter2 text here",
				ScriptId = -1,
			};
			InsertFighter(fighter1);

			var fighter2 = new FighterModel()
			{
				Amount = 4,
				CanMoveOverOpposing = false,
				DeckId = deck.Id,
				Id = -1,
				ImagePath = null,
				IsRanged = false,
				IsSidekick = true,
				IsSmall = true,
				MaxHealth = 1,
				MeleeRange = 1,
				Movement = 3,
				Name = "fighter3",
				StartingHealth = 1,
				Text = "small fighter3 text here",
				ScriptId = -1,
			};
			InsertFighter(fighter2);

			var card1 = new CardModel()
			{
				Id = -1,
				DeckId = deck.Id,
				ScriptId = -1,
				AllowedFighters = "Fighter1,Fighter2",
				Labels = "Label1,label2",
				Boost = -1,
				Count = 2,
				ImagePath = null,
				Name = "c1",
				StartingHandCount = 0,
				Text = "c1 text here",
				Title = "Card1",
				Type = CardModelType.Defense,
				Value = 5,
			};
			InsertCard(card1);

			var cardId = (int)(long)LastInsertedId(card1.SQLTableName());
			var c = GetCard(cardId);
			var script = GetScriptModel(c.ScriptId);
			script.GraphState = graphState1.ToJson();
			UpdateScriptById(script);

			var card2 = new CardModel()
			{
				Id = -1,
				DeckId = deck.Id,
				ScriptId = -1,
				AllowedFighters = "Fighter1",
				Labels = "Label1",
				Boost = 3,
				Count = 20,
				ImagePath = null,
				Name = "c2",
				StartingHandCount = 1,
				Text = "c2 text here",
				Title = "Card2",
				Type = CardModelType.Versatile,
				Value = 3,
			};
			InsertCard(card2);
		}

		GD.Print("Inserted dummy data");
	}

	private void InsertModel<T>(T model) where T : IModel
	{
		var insert = model.SQLInsert();
		var compiled = SQL_COMPILER.Compile(insert).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}

	private void DeleteModel<T>(T model) where T : IModel
	{
		var insert = model.SQLDelete();
		var compiled = SQL_COMPILER.Compile(insert).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}
 
	public object LastInsertedId(string tableName)
	{
		var comm = new SqliteCommand($"SELECT last_insert_rowid() FROM {tableName}", _connection);
		return comm.ExecuteScalar();
	}

	private List<T> QueryMany<T>(Query query, Func<SqliteDataReader, T> converter) where T : IModel
	{
		List<T> result = [];

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		var reader = comm.ExecuteReader();
		while (reader.Read())
		{
			result.Add(converter(reader));
		}

		return result;
	}

	private T QuerySingle<T>(Query query, Func<SqliteDataReader, T> converter) where T : IModel
	{
		var result = QueryMany(query.Limit(1), converter);
		return result.SingleOrDefault();
	}

	#region App state

	public AppStateModel GetAppState()
	{
		return QuerySingle(
			AppStateModel.SQLSelect(),
			AppStateModel.SQLConverter
		);
	}

	public void UpdateAppState(AppStateModel appState)
	{
		var query = (appState as IModel).SQLUpdate();

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}

	#endregion

	#region Decks

	public void InsertDeck(DeckModel deck) => InsertModel(deck);

	public List<DeckModel> GetDecks(bool pickEditableDecks)
	{
		return QueryMany(
			DeckModel.SQLSelect().Where("editable", pickEditableDecks),
			DeckModel.SQLConverter
		);
	}

	public DeckModel GetDeck(int deckId)
	{
		return QuerySingle(
			DeckModel.SQLSelect().Where("id", deckId),
			DeckModel.SQLConverter
		);
	}

	public List<string> GetDeckNames() =>
	[
		.. QueryMany(
			DeckModel.SQLSelect(),
			DeckModel.SQLConverter
		).Select(d => d.Name)
	];

	public DeckModel GetDeck(string deckName) => QuerySingle(
		DeckModel.SQLSelect().Where("name", deckName),
		DeckModel.SQLConverter
	);

	public void UpdateDeckById(DeckModel deck)
	{
		var query = (deck as IModel).SQLUpdate().Where("id", deck.Id);

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}

	#endregion

	#region Fighters

	public void InsertFighter(FighterModel fighter)
	{
		InsertFighter(fighter, "function _Create()\n\t-- TODO manually edit script\nend", true);
	}

	public void InsertFighter(FighterModel fighter, string script, bool isManual)
	{
		// insert script
		var newScript = new ScriptModel()
		{
			Id = -1,
			IsManual = isManual,
			Script = script,
			GraphState = ScriptState.NewFighterScript().ToJson(),
		};

		InsertFighterScript(newScript);
		var scriptId = (long)LastInsertedId(newScript.SQLTableName());

		// insert fighter
		fighter.ScriptId = (int)scriptId;
		InsertModel(fighter);
	}

	public List<FighterModel> GetFighters(int deckId)
	{
		return QueryMany(
			FighterModel.SQLSelect().Where("deck_id", deckId),
			FighterModel.SQLConverter
		);
	}

	public List<string> GetFighterNames(int deckId) =>
	[
		.. GetFighters(deckId).Select(d => d.Name)
	];

	public FighterModel GetFighter(string fighterName, int deckId)
	{
		return QuerySingle(
			FighterModel.SQLSelect()
				.Where("name", fighterName)
				.Where("deck_id", deckId),
			FighterModel.SQLConverter
		);
	}

	public FighterModel GetFighter(int fighterId)
	{
		return QuerySingle(
			FighterModel.SQLSelect().Where("id", fighterId),
			FighterModel.SQLConverter
		);
	}

	public void UpdateFighterById(FighterModel fighter)
	{
		var query = (fighter as IModel).SQLUpdate().Where("id", fighter.Id);

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}

	#endregion

	#region Cards

	public void InsertCard(CardModel card)
	{
		InsertCard(card, "function _Create()\n\t-- TODO manually edit script\nend", false);
	}

	public void InsertCard(CardModel card, string script, bool isManual)
	{
		// insert script
		var newScript = new ScriptModel()
		{
			Id = -1,
			IsManual = isManual,
			Script = script,
			GraphState = ScriptState.NewCardScript().ToJson(),
		};

		InsertCardScript(newScript);
		var scriptId = (long)LastInsertedId(newScript.SQLTableName());

		// insert card
		card.ScriptId = (int)scriptId;
		InsertModel(card);
	}

	public List<CardModel> GetCards(int deckId)
	{
		return QueryMany(
			CardModel.SQLSelect().Where("deck_id", deckId),
			CardModel.SQLConverter
		);
	}

	public List<string> GetCardNames(int deckId) =>
	[
		.. GetCards(deckId).Select(c => c.Name)
	];

	public CardModel GetCard(string cardName, int deckId)
	{
		return QuerySingle(
			CardModel.SQLSelect()
				.Where("name", cardName)
				.Where("deck_id", deckId),
			CardModel.SQLConverter
		);
	}

	public CardModel GetCard(int cardId)
	{
		return QuerySingle(
			CardModel.SQLSelect().Where("id", cardId),
			CardModel.SQLConverter
		);
	}

	public void UpdateCardById(CardModel card)
	{
		var query = (card as IModel).SQLUpdate().Where("id", card.Id);

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}

	#endregion

	#region Scripts

	public void InsertFighterScript(ScriptModel script)
	{
		InsertModel(script);
		var scriptId = (int)(long)LastInsertedId(script.SQLTableName());

		// TODO
	}

	public void InsertCardScript(ScriptModel script)
	{
		InsertModel(script);
		var scriptId = (int)(long)LastInsertedId(script.SQLTableName());

		// TODO
	}

	public ScriptModel GetScriptModel(int scriptId)
	{
		return QuerySingle(
			ScriptModel.SQLSelect().Where("id", scriptId),
			ScriptModel.SQLConverter
		);
	}

	public void UpdateScriptById(ScriptModel script)
	{
		var query = (script as IModel).SQLUpdate().Where("id", script.Id);

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}


	#endregion

	#region Core

	public void UpdateCore(CoreModel core)
	{
		var query = (core as IModel).SQLUpdate();

		var compiled = SQL_COMPILER.Compile(query).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
	}

	public CoreModel GetCore()
	{
		return QuerySingle(
			CoreModel.SQLSelect(),
			CoreModel.SQLConverter
		);
	}

	#endregion

	#region Images

	public string UpdateDeckCardBack(int deckId, string pathToImage)
	{
		var ext = Path.GetExtension(pathToImage);
		var target = Path.Join(CardBackDirectory(), $"{deckId}{ext}");

		DirAccess.CopyAbsolute(
			pathToImage,
			target
		);

		var deck = GetDeck(deckId);
		deck.CardBackPath = target;
		UpdateDeckById(deck);

		return target;
	}

	public string UpdateFighterImage(int fighterId, string pathToImage)
	{
		var ext = Path.GetExtension(pathToImage);
		var target = Path.Join(GetFighterImageDirectory(), $"{fighterId}{ext}");

		DirAccess.CopyAbsolute(
			pathToImage,
			target
		);

		var fighter = GetFighter(fighterId);
		fighter.ImagePath = target;
		UpdateFighterById(fighter);

		return target;
	}

	public string UpdateCardImage(int cardId, string pathToImage)
	{
		var ext = Path.GetExtension(pathToImage);
		var target = Path.Join(GetCardImageDirectory(), $"{cardId}{ext}");

		DirAccess.CopyAbsolute(
			pathToImage,
			target
		);

		var card = GetCard(cardId);
		card.ImagePath = target;
		UpdateCardById(card);

		return target;
	}

	#endregion

	public void ProcessContentUpdate(ContentUpdateGet contentUpdate)
	{
		GD.Print(contentUpdate.Loadouts.Count);
		// remove non-editable content
		var decks = GetDecks(false);
		foreach (var deck in decks)
		{
			// cards
			var cards = GetCards(deck.Id);
			foreach (var card in cards)
			{
				DeleteModel(card);
				DeleteModel(GetScriptModel(card.ScriptId));
			}

			// fighters
			var fighters = GetFighters(deck.Id);
			foreach (var fighter in fighters)
			{
				DeleteModel(fighter);
				DeleteModel(GetScriptModel(fighter.ScriptId));
			}

			DeleteModel(deck);
		}

		// insert new content
		foreach (var deck in contentUpdate.Loadouts)
		{
			var newDeck = new DeckModel()
			{
				Id = -1,
				CardBackPath = null, // TODO
				ChoosesSidekick = deck.ChoosesSidekick,
				Description = "", // TODO
				Editable = false,
				MaxHandSize = deck.MaximumHandSize,
				Name = deck.Name,
				StartingHandSize = deck.StartingHandSize,
				StartsWithSidekicks = deck.StartsWithSidekicks
			};
			InsertDeck(newDeck);
			var deckId = (int)(long)LastInsertedId(newDeck.SQLTableName());

			// fighters
			foreach (var fighter in deck.Fighters)
			{
				var newFighter = new FighterModel()
				{
					Id = -1,
					DeckId = deckId,	
					Amount = fighter.Amount,
					CanMoveOverOpposing = fighter.CanMoveOverOpposing,
					ImagePath = null, // TODO
					IsRanged = fighter.IsRanged,
					IsSidekick = !fighter.IsHero,
					IsSmall = fighter.IsSmall,
					MaxHealth = fighter.MaxHealth,
					MeleeRange = fighter.MeleeRange,
					Movement = fighter.Movement,
					Name = fighter.Name,
					StartingHealth = fighter.StartingHealth,
					Text = fighter.Text,
					ScriptId = -1
				};

				InsertFighter(newFighter, fighter.Script, true);
			}

			// Cards
			foreach (var card in deck.Deck)
			{
				var newCard = new CardModel()
				{
					Id = -1,
					DeckId = deckId,	
					AllowedFighters = CardModel.ToAllowedFighters([.. card.AllowedFighters]),
					Labels = CardModel.ToLabels([.. card.Labels]),
					Boost = card.Boost ?? -1,
					Count = card.Amount,
					ImagePath = null, // TODO
					Name = card.Name,
					Text = card.Text,
					Title = card.Name,
					Type = card.Type switch // TODO this shouldn't be here
					{
						"Attack" => CardModelType.Attack,
						"Defense" => CardModelType.Defense,
						"Versatile" => CardModelType.Versatile,
						"Scheme" => CardModelType.Scheme,
						_ => throw new Exception($"Received unrecognized card type: {card.Type}")
					},
					StartingHandCount = deck.StartsWithCards.Count(k => k == card.Key),
					Value = card.Value ?? 0,
					ScriptId = -1
				};

				InsertCard(newCard, card.Script, true);
				// TODO change script
			}
		}

		// core script
		var core = GetCore();
		if (core is null)
		{
			core = new CoreModel()
			{
				Id = -1,
				Text = contentUpdate.Core
			};
			InsertModel(core);
			return;
		}
		core.Text = contentUpdate.Core;
		UpdateCore(core);

		EmitSignalContentUpdateProcessed();
	}

	public Godot.Collections.Dictionary<string, Texture2D> GetFighterTextureMap(int deckId)
	{
		var deck = GetDeck(deckId);
		var fighters = GetFighters(deckId);
		Godot.Collections.Dictionary<string, Texture2D> result = [];
		foreach (var fighter in fighters)
		{
			var key = $"{deck.Name}_{fighter.Name}";
			var texture = GD.Load<Texture2D>(fighter.ImagePath);
			result.Add(key, texture);
		}
		return result;
	}

	public Godot.Collections.Dictionary<string, Texture2D> GetCardTextureMap(int deckId)
	{
		var deck = GetDeck(deckId);
		var cards = GetCards(deckId);
		Godot.Collections.Dictionary<string, Texture2D> result = [];
		foreach (var card in cards)
		{
			var key = $"{deck.Name}_{card.Name}";
			var texture = GD.Load<Texture2D>(card.ImagePath);
			result.Add(key, texture);
		}
		return result;
	}

	public Texture2D GetCardBackTexture(int deckId)
	{
		var deck = GetDeck(deckId);
		return GD.Load<Texture2D>(deck.CardBackPath);
	}

	public LoadoutTemplate GetLoadoutTemplate(int deckId)
    {
		var deck = GetDeck(deckId);
        var result = new LoadoutTemplate()
        {
            Name = deck.Name,
            StartsWithSidekicks = deck.StartsWithSidekicks,
            ChoosesSidekick = deck.ChoosesSidekick,
            StartingHandSize = deck.StartingHandSize,
            MaximumHandSize = deck.MaxHandSize,
            CantBePlayedWith = [], // TODO?
            StartsWithCards = [],
            Deck = [],
            Fighters = []
        };

		// fighters
		var fighters = GetFighters(deckId);
		foreach (var fighter in fighters)
		{
			var script = GetScriptModel(fighter.ScriptId);

			result.Fighters.Add(new()
			{
				Name = fighter.Name,
				Amount = fighter.Amount,
				CanMoveOverOpposing = fighter.CanMoveOverOpposing,
				IsHero = !fighter.IsSidekick,
				IsRanged = fighter.IsRanged,
				IsSmall = fighter.IsSmall,
				Key = $"{deck.Name}_{fighter.Name}",
				MaxHealth = fighter.MaxHealth,
				MeleeRange = fighter.MeleeRange,
				Movement = fighter.Movement,
				Script = script.Script,
				StartingHealth = fighter.StartingHealth,
				Text = fighter.Text
			});		
		}

		// fighters
		var cards = GetCards(deckId);
		foreach (var card in cards)
		{
			var script = GetScriptModel(card.ScriptId);
			var ct = new CardTemplate()
			{
				Name = card.Title,
				Key = $"{deck.Name}_{card.Name}",
				AllowedFighters = card.GetAllowedFighters(),
				Labels = card.GetLabels(),
				Amount = card.Count,
				Boost = card.Boost,
				IncludedInDeckWithSidekick = null, // TODO
				Script = script.Script,
				Text = card.Text,
				Type = card.Type.ToCardTemplateType(),
				Value = card.Value,
			};

			for (int i = 0; i < card.StartingHandCount; ++i)
			{
				result.StartsWithCards.Add(ct.Key);
			}

			result.Deck.Add(ct);
		}

		return result;
	}
}
