using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UMDTO;
using UMServer.BusinessLogic;
using UMServer.Repositories;
using UMServer.Services;

namespace UMServer.Hubs;

public class MatchesHub(
    ILogger<MatchesHub> logger,
    IMatchManager matchesManager,
    IClientRepository clientRepo,
    ILoadoutRepository loadoutRepo,
    ICoreScriptRepository coreRepo,
    IMatchConnectEndpointSerializer connectSerializer
) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        logger.LogDebug("New connection! ConnectionId: {}. Waiting for name", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);

        logger.LogDebug("Client with Id = {} was disconnected", Context.ConnectionId);

        var client = await clientRepo.Get(Context.ConnectionId);
        if (client is null)
        {
            return;
        }
        await clientRepo.Remove(client);
        await matchesManager.ProcessRemovedClient(client);
    }

    public async Task<string> RegisterName(string name)
    {
        logger.LogDebug("Client with Id = {} tries to register with name {}", Context.ConnectionId, name);

        var errMsg = await clientRepo.Add(Context.ConnectionId, name);

        return errMsg;
    }

    public async Task<string> CreateMatch(CreateMatchParams createParams)
    {
        var creator = await clientRepo.Get(Context.ConnectionId);
        if (creator is null)
        {
            return "err:You are not registered (somehow)";
        }

        logger.LogDebug("User {} wants to create a match with title {}", creator.Name, createParams.Title);

        var match = await matchesManager.Create(creator, createParams);

        if (match is null)
        {
            return "err:Failed to create match";
        }

        return match.Id;
    }

    public async Task PublishToChat(string matchId, string msg)
    {
        var client = await clientRepo.Get(Context.ConnectionId);
        if (client is null)
        {
            return;
        }

        var match = await matchesManager.Get(matchId);
        if (match is null)
        {
            return;
        }

        await BroadcastUserMessage(matchId, client.Name, msg);
    }

    private async Task BroadcastChatMessage(string matchId, ChatMessage msg)
    {
        await Clients.Group(matchId).SendAsync("ChatUpdate", JsonSerializer.Serialize(msg));
    }

    private async Task BroadcastSystemMessage(string matchId, string msg)
    {
        await BroadcastChatMessage(matchId, new()
        {
            From = "",
            Msg = msg
        });
    }

    private async Task BroadcastUserMessage(string matchId, string playerName, string msg)
    {
        await BroadcastChatMessage(matchId, new()
        {
            From = playerName,
            Msg = msg
        });
    }

    public async Task<string> Connect(string matchId) // TODO add password
    {
        var client = await clientRepo.Get(Context.ConnectionId);
        if (client is null)
        {
            return "err:You are not registered (somehow)";
        }

        var match = await matchesManager.Get(matchId);
        if (match is null)
        {
            return "err:Match not found";
        }
        if (match.Status != Matches.MatchProcessStatus.WAITING_FOR_PLAYERS)
        {
            return "err:Match doesnt accept connections";
        }

        if (match.GetConnectedPlayer(client.Id) is not null)
        {
            return "err:Already connected to this match";
        }
        await match.ConnectClient(client);
        
        logger.LogDebug("Client {} connects to match {}", client.Id, match.Id);

        // add to chat
        await Groups.AddToGroupAsync(Context.ConnectionId, matchId);
        await BroadcastSystemMessage(matchId, $"Player {client.Name} connected!");

        return connectSerializer.Serialize(client.Id, match);
    }

    // TODO
    // public async Task Kick(string matchId, string clientName)
    // {
        
    // }

    public async Task<string> SelectLoadout(string matchId, string loadoutName)
    {
        var match = await matchesManager.Get(matchId);
        if (match is null)
        {
            return "Match not found";
        }

        var player = match.GetConnectedPlayer(Context.ConnectionId);
        if (player is null)
        {
            return "You are not part of this match";
        }

        var loadout = await loadoutRepo
            .Query()
            .Where(l => l.Name == loadoutName)
            .SingleOrDefaultAsync();

        if (loadout is null)
        {
            return $"Unknown loadout {loadoutName}";
        }

        logger.LogDebug("Client {} in match {} sets their loadout to {}", player.ClientId, match.Id, loadout.Name);

        await player.SetLoadout(loadout);

        return "";
    }

    public async Task<string> SelectTeam(string matchId, int teamIdx)
    {
        var match = await matchesManager.Get(matchId);
        if (match is null)
        {
            return "Match not found";
        }

        var player = match.GetConnectedPlayer(Context.ConnectionId);
        if (player is null)
        {
            return "You are not part of this match";
        }

        if (teamIdx >= match.Config.TeamCount)
        {
            return "Selected team exceeds team count";
        }

        logger.LogDebug("Client {} in match {} sets their team to {}", player.ClientId, match.Id, teamIdx);

        await player.SetTeamIdx(teamIdx);

        return "";
    }

    public async Task<bool> CanStart(string matchId)
    {
        var match = await matchesManager.Get(matchId);
        if (match is null)
        {
            return false;
        }

        var player = match.GetConnectedPlayer(Context.ConnectionId);
        if (player is null)
        {
            return false;
        }

        return match.CanStart();
    }

    public async Task Start(string matchId)
    {
        var match = await matchesManager.Get(matchId);
        if (match is null)
        {
            return;
        }
        if (!match.CanStart())
        {
            return;
        }

        var player = match.GetConnectedPlayer(Context.ConnectionId);
        if (player is null)
        {
            return;
        }

        if (match.OwnerId != player.ClientId)
        {
            return;
        }

        if (match.Status != Matches.MatchProcessStatus.WAITING_FOR_PLAYERS)
        {
            return;
        }

        await match.TryRun(
            coreRepo,
            clientRepo
        );
    }
}

public class ChatMessage
{
    public required string From { get; init; }
    public required string Msg { get; init; }
}