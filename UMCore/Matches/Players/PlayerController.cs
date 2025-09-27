using System.Text.Json;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players;

public interface IPlayerController
{
    void AddEvent(Event e);
    void AddLog(Log l);
    
    Task Setup(Player player, Match.SetupData setupData);
    Task Update(Player player);
    Task<string> ChooseAction(Player player, string[] options);
    Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint);
    Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint);
    Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint);
    Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint);
    Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options);
    Task<string> ChooseString(Player player, string[] options, string hint);
    Task<Player> ChoosePlayer(Player player, Player[] options, string hint);
}

public class RandomPlayerController(int seed) : IPlayerController
{
    private readonly Random _rnd = new(seed);

    public void AddEvent(Event e)
    {
    }

    public void AddLog(Log l)
    {
    }

    // private readonly Random _rnd = new();

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        if (options.Contains("Fight")) return "Fight";
        return options[_rnd.Next(options.Length)];
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        if (options.Length == 0) return null;
        if (_rnd.Next(2) == 0) return null;
        return await ChooseCardInHand(player, playerHandIdx, options, hint);
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public Task Setup(Player player)
    {
        return Task.CompletedTask;
    }

    public Task Setup(Player player, Match.SetupData setupData)
    {
        return Task.CompletedTask;
    }

    public Task Update(Player player)
    {
        return Task.CompletedTask;
    }
}

public class DelayedControllerWrapper(TimeSpan delay, IPlayerController controller) : IPlayerController
{
    public void AddEvent(Event e)
    {
    }

    public void AddLog(Log l)
    {
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        await Task.Delay(delay);
        return await controller.ChooseAction(player, options);
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        await Task.Delay(delay);
        return await controller.ChooseAttack(player, options);
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChooseCardInHand(player, playerHandIdx, options, hint);
    }

    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChooseCardInHandOrNothing(player, playerHandIdx, options, hint);
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChooseFighter(player, options, hint);
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChooseNode(player, options, hint);
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChoosePlayer(player, options, hint);
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChooseString(player, options, hint);
    }

    public async Task Setup(Player player, Match.SetupData setupData)
    {
        await controller.Setup(player, setupData);
    }

    public async Task Update(Player player)
    {
        await Task.Delay(delay);
        await controller.Update(player);
    }
}