using System.Net.WebSockets;
using Microsoft.AspNetCore.SignalR;
using UMDTO;
using UMServer.Hubs;
using UMServer.Matches;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface IMatchManager
{
    Task<MatchProcess?> Create(ConnectedClient client, CreateMatchParams createParams);
    Task ProcessRemovedClient(ConnectedClient client);
    Task<string> WSTryConnect(WebSocketManager wsm, string connectionId, string matchId);
    Task<MatchProcess?> Get(string matchId);
    Task UpdateWatchers();
    Task UpdateWatcher(string clientId);
    Task<MatchRecordGet> GetRecord(string matchId);
    Task<IEnumerable<MatchProcessGet>> All();
}

public class MatchManager(
    ILogger<MatchManager> logger,
    IMatchRepository matchRepo,
    IClientRepository clientRepo,
    IMatchConfigRepository configRepo,
    IHubContext<MatchesHub> matchesHub
) : IMatchManager
{
    public Task<IEnumerable<MatchProcessGet>> All() => Task.FromResult(
        matchRepo.All().Select(m => m.ToMatchProcessGet()
    ));
    
    public async Task UpdateWatchers()
    {
        await matchesHub.Clients.All.SendAsync(
            "UpdateTables",
            matchRepo.All().Select(m => m.ToMatchProcessGet())
        );
    }

    public async Task UpdateWatcher(string clientId)
    {
        await matchesHub.Clients.Client(clientId).SendAsync(
            "UpdateTables",
            matchRepo.All().Select(m => m.ToMatchProcessGet())
        );
    }

    public async Task<MatchProcess?> Create(ConnectedClient client, CreateMatchParams createParams)
    {
        var config = await configRepo.ByName(createParams.MatchConfigName);
        if (config is null)
        {
            return null;
        }

        var match = new MatchProcess(
            Guid.NewGuid().ToString(),
            client.Id,
            config,
            createParams
        );
        match.OnChanged += UpdateWatchers;

        matchRepo.Add(match);

        logger.LogDebug("Created new match with id = {}, current match count: {}", match.Id, matchRepo.Count());

        await UpdateWatchers();

        return match;
    }

    public async Task ProcessRemovedClient(ConnectedClient client)
    {
        var matches = matchRepo
            .All()
            .Where(p => p.HasClient(client.Id))
            .ToList();

        foreach (var match in matches)
        {
            if (match.IsFinished()) continue;
            await match.ForceStop();
            await RemoveMatch(match);
            logger.LogDebug("Removed match with id = {} due to disconnected player", match.Id);
        }
    }

    public async Task RemoveMatch(MatchProcess match)
    {
        matchRepo.Remove(match);
        await UpdateWatchers();
    }

    public async Task<string> WSTryConnect(WebSocketManager wsm, string connectionId, string matchId)
    {
        var client = await clientRepo.Get(connectionId);
        if (client is null)
        {
            throw new Exception($"Unregistered client with Id = {connectionId} tried to connect to a match");
        }

        var match = matchRepo.Get(matchId);
        if (match is null)
        {
            return $"Match with Id = {matchId} does not exist";
        }
        if (match.Status != MatchProcessStatus.WAITING_FOR_PLAYERS)
        {
            return $"Match does not accept players";
        }

        var player = match.GetConnectedPlayer(client.Id);
        if (player is null)
        {
            return $"You are not connected to the match";
        }

        var socket = await wsm.AcceptWebSocketAsync();

        logger.LogDebug("Client {} connects to match {}", client.Name, match.CreateParams.Title);

        var matchEnd = await match.SetPlayerSocket(
            player,
            socket
        );

        // await match.TryRun(
        //     logger,
        //     coreRepo,
        //     clientRepo
        // );

        await matchEnd.Task;

        return string.Empty;
    }

    public async Task<MatchProcess?> Get(string matchId)
    {
        return matchRepo.Get(matchId);
    }

    public Task<MatchRecordGet> GetRecord(string matchId)
    {
        var match = matchRepo.Get(matchId)
            ?? throw new MatchNotFoundException(matchId);
        if (!match.IsFinished())
        {
            throw new MatchNotFinishedException(matchId);
        }
        if (match.Record is null)
        {
            throw new Exception("Match is finished yet has no record");
        }
        return Task.FromResult(
            match.Record.ToMatchRecordGet()
        );
    }
}

[Serializable]
public class MatchNotFinishedException(string matchId)
: Exception($"Match with Id = {matchId} is not finished yet")
{ }

[Serializable]
public class MatchNotFoundException(string matchId)
: Exception($"Match with Id = {matchId} doesnt exist")
{ }

