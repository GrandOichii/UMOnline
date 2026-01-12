using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Microsoft.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;
using UMCore.Matches.Players.Cards;

public partial class LocalRepository : Node
{
	private static readonly SqliteCompiler SQL_COMPILER = new();

	[Export]
	public string DataSource { get; set; }
	[Export]
	public bool DropOnLaunch { get; set; }

	private readonly List<IModel> _defaultModels = [
		DeckModel.Default,
		CardModel.Default,
		FighterModel.Default,
		ScriptModel.Default,
		ScriptNodeModel.Default,
	];

	private SqliteConnection _connection;

	public override void _Ready()
	{
		var path = ProjectSettings.GlobalizePath(DataSource);
		// GD.Print(path);
		_connection = new SqliteConnection($"Data Source={path}");
		_connection.Open();

		// drop tables
		if (DropOnLaunch)
		{
			foreach (var model in _defaultModels)
			{
				ExecNonQuery(model.SQLDrop());
			}
		}

		// create tables
		foreach (var model in _defaultModels)
		{
			ExecNonQuery(model.SQLCreate());
		}

		GD.Print("Tables created!");

		InsertDummyData();
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
			}
		];

		foreach (var deck in decks)
		{
			InsertModel(deck);
		}

		GD.Print("Inserted dummy data");
	}

	public void InsertModel<T>(T model) where T : IModel
	{
		var insert = model.SQLInsert();
		var compiled = SQL_COMPILER.Compile(insert).ToString();
		var comm = new SqliteCommand(compiled, _connection);
		comm.ExecuteNonQuery();
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
}
