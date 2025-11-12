namespace UMClient.Storage;

using Godot;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using UMClient.Storage.Models;

public partial class LoadoutStorage : Node
{
	[Export]
	public string DataSource { get; set; }

	private SqliteConnection _connection;

	public override void _Ready()
	{
		_connection = new SqliteConnection($"Data Source={DataSource}");
		_connection.Open();

		TryCreateDb();
	}

	private void TryCreateDb() {
		List<string> createCommands = [
			Loadout.GetCreateCommand(),
		];
		try {
			foreach (var createCommand in createCommands)
			{
				var command = new SqliteCommand(createCommand, _connection);
				command.ExecuteNonQuery();
			}
		} catch (SqliteException e) {
			GD.PushWarning(e.Message);
		}
	}

}
