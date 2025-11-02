using System.Diagnostics.CodeAnalysis;
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
    Task<Path> ChoosePath(Player player, Path[] options, string hint);
}

public class SafePlayerController(IPlayerController controller) : IPlayerController
{
    public IPlayerController Controller { get; } = controller;

    [Serializable]
    public class UnsafeChoiceException(string message) : Exception(message) { }

    public void AddEvent(Event e)
    {
        Controller.AddEvent(e);
    }

    public void AddLog(Log l)
    {
        Controller.AddLog(l);
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        var result = await Controller.ChooseAction(player, options);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result} for {nameof(ChooseAction)}, which is not one of the options (options: {string.Join(", ", options)})");
        }
        return result;
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var result = await Controller.ChooseAttack(player, options);
        if (!options.Contains(result))
        {
            // TODO add better exception message
            throw new UnsafeChoiceException($"Player {player.LogName} chose an invalid attack for {nameof(ChooseAttack)}");
        }
        return result;
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        var result = await Controller.ChooseCardInHand(player, playerHandIdx, options, hint);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result.LogName} for {nameof(ChooseCardInHand)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.LogName))})");
        }
        return result;
    }

    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        var result = await Controller.ChooseCardInHandOrNothing(player, playerHandIdx, options, hint);
        if (result is not null && !options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result.LogName} for {nameof(ChooseCardInHandOrNothing)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.LogName))})");
        }
        return result;
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var result = await Controller.ChooseFighter(player, options, hint);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result.LogName} for {nameof(ChooseFighter)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.LogName))})");
        }
        return result;
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var result = await Controller.ChooseNode(player, options, hint);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result.Id} for {nameof(ChooseNode)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.Id))})");
        }
        return result;
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var result = await Controller.ChoosePlayer(player, options, hint);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result.LogName} for {nameof(ChoosePlayer)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.LogName))})");
        }
        return result;
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var result = await Controller.ChooseString(player, options, hint);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result} for {nameof(ChoosePlayer)}, which is not one of the options (options: {string.Join(", ", options)})");
        }
        return result;
    }

    public async Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        var result = await Controller.ChoosePath(player, options, hint);
        if (!options.Contains(result))
        {
            throw new UnsafeChoiceException($"Player {player.LogName} tried to choose {result} for {nameof(ChoosePath)}, which is not one of the options");
        }
        return result;
    }


    public Task Setup(Player player, Match.SetupData setupData)
    {
        return Controller.Setup(player, setupData);
    }

    public Task Update(Player player)
    {
        return Controller.Update(player);
    }

}

public class RandomPlayerController(int seed) : IPlayerController
{
    private readonly Random _rnd = new(seed);
    // private readonly Random _rnd = new();

    public void AddEvent(Event e)
    {
    }

    public void AddLog(Log l)
    {
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

    public Task<string> ChooseAction(Player player, string[] options)
    {
        if (options.Contains("Fight")) return Task.FromResult("Fight");
        return Task.FromResult(options[_rnd.Next(options.Length)]);
    }

    public Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
    }

    public Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
    }

    public Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        var idx = _rnd.Next(options.Length + 1);
        if (idx == 0) return Task.FromResult<MatchCard?>(null);
        return Task.FromResult((MatchCard?)options[idx - 1]);
    }

    public Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
    }

    public Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
    }

    public Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
    }

    public Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
    }

    public Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var opts = options.ToList();
        return Task.FromResult(opts[_rnd.Next(opts.Count)]);
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

    public async Task Setup(Player player, Match.SetupData setupData)
    {
        await controller.Setup(player, setupData);
    }

    public async Task Update(Player player)
    {
        await Task.Delay(delay);
        await controller.Update(player);
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

    public async Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        await Task.Delay(delay);
        return await controller.ChoosePath(player, options, hint);
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
}