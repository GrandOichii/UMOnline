using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Controllers;

public class LocalMatchIOHandler(LocalMatch match) : IIOHandler
{
	private TaskCompletionSource<string> _readTask = null;
	public void SetReadTaskResult(string result)
	{
		_readTask.SetResult(result);
	}

	public Task Close()
	{
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
