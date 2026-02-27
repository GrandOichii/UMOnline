using System.Net.WebSockets;
using UMDTO;
using UMModel.Models;
using UMServer.BusinessLogic;
using UMServer.Repositories;

namespace UMServer.Matches;

public enum MatchProcessStatus
{
    WAITING_FOR_PLAYERS,
    IN_PROGRESS,
    FINISHED,
    CRASHED
}

public class MatchProcess(
    string id,
    string ownerId,
    MatchConfig config,
    CreateMatchParams createParams
)
{
	public static readonly Random RND = new();

    public string Id { get; } = id;
    public MatchProcessStatus Status { get; private set; } = MatchProcessStatus.WAITING_FOR_PLAYERS;
    public string OwnerId { get; } = ownerId; 
    public CreateMatchParams CreateParams { get; } = createParams;
    public MatchConfig Config { get; } = config;
    public List<ConnectedPlayer> Players { get; } = [];
    private UMCore.Matches.Match? _match = null;
	private readonly TaskCompletionSource _matchEndTask = new();
	public Exception? MatchException { get; private set; } = null;
	public MatchRecord? Record { get; private set; }

	public delegate Task MatchProcessChanged();
	public event MatchProcessChanged? OnChanged;

	public bool IsFinished() => Status == MatchProcessStatus.CRASHED || Status == MatchProcessStatus.FINISHED;
    
    public bool HasClient(string connectionId)
    {
        // just in case
        if (OwnerId == connectionId)
        {
            return true;
        }

        foreach (var player in Players)
        {
            if (player.Client.Id == connectionId)
            {
                return true;
            }
        }

        return false;
    }

    public async Task ForceStop()
    {
        foreach (var player in Players)
        {
            if (player.Socket is null) continue;
            await player.Socket.CloseAsync(
                System.Net.WebSockets.WebSocketCloseStatus.NormalClosure,
                "One of the players was disconnected",
                CancellationToken.None
            );
        }
    }

    public async Task ConnectClient(ConnectedClient client)
    {
        if (Status != MatchProcessStatus.WAITING_FOR_PLAYERS) return;
        var player = new ConnectedPlayer(
            client
        );

        Players.Add(player);

		if (OnChanged is not null)
			await OnChanged.Invoke();
    }

    public ConnectedPlayer? GetConnectedPlayer(string clientId)
    {
        return Players.SingleOrDefault(p => p.Client.Id == clientId);
    }

    public bool CanStart()
    {
        var qpc = new UMCore.Matches.QueuedPlayerCollection(Config);
        foreach (var player in Players)
        {
            if (player.Loadout is null) return false;
            qpc.AddPlayer(player.Client.Id, player.TeamIdx, player.Loadout.ToTemplate());
        }
        return qpc.CanRun();
    }

    private async Task SetStatus(MatchProcessStatus value)
    {
        Status = value;

		if (OnChanged is not null)
			await OnChanged.Invoke();
    }

	public async Task<TaskCompletionSource> SetPlayerSocket(ConnectedPlayer player, WebSocket socket)
	{
		await player.SetSocket(socket);
		return _matchEndTask;
	}

    public async Task TryRun(
        ICoreScriptRepository coreRepo,
        IClientRepository clientRepo)
    {
		if (Status != MatchProcessStatus.WAITING_FOR_PLAYERS) {
			return;
		}
        if (Players.Any(
            p => p.Socket is null
        )) {
			// logger.LogDebug("Some players havent connected yet");
			return;
		}

        await SetStatus(MatchProcessStatus.IN_PROGRESS);

		var seed = RND.Next();
        var core = await coreRepo.Active(); 

		var config = new UMCore.Matches.MatchConfig()
		{
			InitialHandSize = Config.InitialHandSize,
			ActionsPerTurn = Config.ActionsPerTurn,
			MaxHandSize = Config.MaxHandSize,
			ManoeuvreDrawAmount = Config.ManoeuvreDrawAmount,
			RandomFirstPlayer = Config.RandomFirstPlayer,
			FirstPlayerIdx = Config.FirstPlayerIdx,
			ExhaustDamage = Config.ExhaustDamage,
			TeamSize = Config.TeamSize,
			TeamCount = Config.TeamCount,
			RandomMatch = false,
			Seed = seed
		};
        _match = new(config,
            GetBaskervilleTemplate(),
            core.Script)
        {
            Logger = null // TODO
            // Logger = logger // TODO
        };

		Record = new(seed, this);

        foreach (var player in Players)
        {
            var client = await clientRepo.Get(player.Client.Id);
            if (client is null)
            {
                throw new Exception($"Tried to start match with unregistered player {player.Client.Id}");
            }
            if (player.Loadout is null)
            {
                throw new Exception($"Called {nameof(TryRun)} with a player missing a loadout");
            }

            var controller = new UMCore.Matches.Players.Controllers.IOPlayerController(
                new WebSocketIOHandler(player.Socket!)
            );

			// logger.LogDebug("Added player {}", client.Name);
            var added = await _match.AddPlayer(
                client.Name,
                player.TeamIdx,
                player.Loadout.ToTemplate(),
                controller
            );

            if (!added)
            {
                throw new Exception("Failed to add player to match, not enough checks");
            }
        }

		// logger.LogDebug("Players added, starting match");

        try
        {

            await _match.Run();
            await SetStatus(MatchProcessStatus.FINISHED);

        } catch (Exception e)
        {
			
			MatchException = e;
            await SetStatus(MatchProcessStatus.CRASHED);
			// Console.WriteLine(e);
        }

		_matchEndTask.SetResult();

        // TODO save match record
    }

	public MatchProcessGet ToMatchProcessGet()
	{
		return new()
		{
			Id = Id,
			Status = (MatchProcessGetStatus)Status,
			Title = CreateParams.Title,
			AllowedFighters = CreateParams.AllowedLoadouts,
			TeamCount = Config.TeamCount,
			Players = [ .. Players.Select(p => new MatchProcessGetPlayer()
			{
				Name = p.Client.Name,
				TeamIdx = p.TeamIdx,
				LoadoutName = p.Loadout?.Name
			})],
		};
	}

    public static IEnumerable<UMCore.Templates.MapNodeLinkTemplate> Bidirectional(UMCore.Templates.MapNodeTemplate n1, UMCore.Templates.MapNodeTemplate n2)
	{
		return [
			new() {
				First = n1.Id,
				Second = n2.Id,
			},
		];
	}
	
	public static UMCore.Templates.MapTemplate GetBaskervilleTemplate()
	{
		List<UMCore.Templates.MapNodeTemplate> nodes = [
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


}