using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UMDTO;
using UMServer.Extensions;
using UMServer.Hubs;
using UMServer.Matches;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface IMatchManager
{
    Task<MatchProcess?> Create(ConnectedClient client, CreateMatchParams createParams);
    Task ProcessRemovedClient(ConnectedClient client);
    Task WSTryConnect(WebSocketManager wsm, string connectionId, string matchId);
    Task<MatchProcess?> Get(string matchId);
    Task UpdateWatchers();
    Task UpdateWatcher(string clientId);
}

public class MatchManager(
    ILogger<MatchManager> logger,
    IMatchRepository matchRepo,
    IClientRepository clientRepo,
    IMatchConfigRepository configRepo,
    IHubContext<MatchesHub> matchesHub
) : IMatchManager
{
    public async Task UpdateWatchers()
    {
        await matchesHub.Clients.All.SendAsync(
            "UpdateTables", 
            matchRepo.Query().Select(m => m.ToMatchProcessGet())
        );
    }

    public async Task UpdateWatcher(string clientId)
    {
        await matchesHub.Clients.Client(clientId).SendAsync(
            "UpdateTables", 
            matchRepo.Query().Select(m => m.ToMatchProcessGet())
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

        logger.LogDebug("Created new match with id = {}, current match count: {}", match.Id, matchRepo.Query().Count());

        await UpdateWatchers();

        return match;
    }

    public async Task ProcessRemovedClient(ConnectedClient client)
    {
        var matches = matchRepo
            .Query()
            .Where(p => p.HasClient(client.Id))
            .ToList();

        foreach (var match in matches)
        {
            // TODO? replace player with AI
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

    public async Task WSTryConnect(WebSocketManager wsm, string connectionId, string matchId)
    {
        var client = await clientRepo.Get(connectionId);
        if (client is null)
        {
            throw new Exception($"Unregistered client with Id = {connectionId} tried to connect to a match");
        }

        var match = matchRepo.Get(matchId);
        if (match is null)
        {
            return;
        }
        if (match.Status != MatchProcessStatus.WAITING_FOR_PLAYERS)
        {
            return;
        }

        var player = match.GetConnectedPlayer(client.Id);
        if (player is null)
        {
            return;
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
    }

    public async Task<MatchProcess?> Get(string matchId)
    {
        return matchRepo.Get(matchId);
    }
}