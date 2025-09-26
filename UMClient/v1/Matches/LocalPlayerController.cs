using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using UMCore;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Cards;

namespace UMClient.v1.Matches;

// public interface LocalPlayerControllerHandler
// {
//     void ChangeToChooseAction(Player player, string[] options);
// }

public class LocalMatchIOHandler(TestMatch match) : IIOHandler
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
		// throw new NotImplementedException();

		return _readTask.Task;
	}

	public async Task Write(UpdateInfo info)
	{
		// match.CallDeferred("Load", info.ToVariant());
		match.CallDeferred("Load", Json.ParseString(JsonSerializer.Serialize(info)));
	}
}
