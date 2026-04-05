using Shouldly;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;

namespace UMCore.Tests.Setup;

public class TestPlayerController : IPlayerController
{
    public static readonly string NEXT_ACTION = "next";


    /// <summary>
    /// Player action
    /// </summary>
    /// <param name="player">Player</param>
    /// <param name="options">Action options</param>
    /// <returns>the action word and whether to remove the action from queue or not</returns>
    public delegate Task<(string, bool)> PlayerAction(TestMatch match, Player player, string[] options);

    public delegate Task<(MatchCard?, bool)> CardChoice(Player player, MatchCard[] options, string hint);
    public delegate (Fighter?, bool) FighterChoice(Player player, Fighter[] options, string hint);
    public delegate (MapNode?, bool) NodeChoice(Player player, MapNode[] options, string hint);
    public delegate (AvailableAttack?, bool) AttackChoice(Player player, AvailableAttack[] options);
    public delegate (string?, bool) StringChoice(Player player, string[] options, string hint);
    public delegate (UMCore.Matches.Path?, bool) PathChoice(Player player, UMCore.Matches.Path[] options, string hint);
    public delegate (PlacedToken?, bool) TokenChoice(Player player, PlacedToken[] options, string hint);

    public required Queue<PlayerAction> Actions { get; init; }
    public required Queue<CardChoice> CardChoices { get; init; }
    public required Queue<FighterChoice> FighterChoices { get; init; }
    public required Queue<NodeChoice> NodeChoices { get; init; }
    public required Queue<AttackChoice> AttackChoices { get; init; }
    public required Queue<StringChoice> StringChoices { get; init; }
    public required Queue<PathChoice> PathChoices { get; init; }
    public required Queue<TokenChoice> TokenChoices { get; init; }

    public bool SetupCalled { get; private set; } = false;

    public void AddEvent(Event e)
    {
    }

    public void AddLog(Log l)
    {
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        var match = (player.Match as TestMatch)!;
        var result = NEXT_ACTION;
        while (result == NEXT_ACTION)
        {
            if (!Actions.TryPeek(out var action))
                throw new Exception($"No actions left in queue for player {player.LogName}");
            bool next;
            (result, next) = await action(match, player, options);
            if (next) Actions.Dequeue();
        }

        if (!options.Contains(result))
        {
            throw new Exception($"Received action \"{result}\", which is not a valid action! (expected: \"{string.Join(", ", options)}\")");
        }

        return result;
    }

    public Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        while (AttackChoices.Count > 0)
        {
            var choice = AttackChoices.Dequeue();
            var (result, isResult) = choice(player, options);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null attack choice for {nameof(ChooseAttack)}");
            return Task.FromResult(result);
        }
        
        throw new Exception($"No attack choices left in queue for player {player.LogName}");
    }

    public async Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        var result = await ChooseCardOrNothing(player, options, hint)
            ?? throw new Exception($"Provided null as a card for {nameof(ChooseCard)} (player: {player.LogName}, hint: {hint})");
        return result;
    }

    public async Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        while (CardChoices.Count > 0)
        {
            var choice = CardChoices.Dequeue();
            var (result, isResult) = await choice(player, options, hint);
            if (!isResult) continue;
            return result;
        }
        
        throw new Exception($"No card choices left in queue for player {player.LogName} (hint: {hint})");
    }

    public Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        while (FighterChoices.Count > 0)
        {
            var choice = FighterChoices.Dequeue();
            var (result, isResult) = choice(player, options, hint);
            if (!isResult) continue;
            return Task.FromResult(result!);
        }
        throw new Exception($"No fighter choices left in queue for player {player.LogName} (hint: {hint})");
    }

    public Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        while (NodeChoices.Count > 0)
        {
            var choice = NodeChoices.Dequeue();
            var (result, isResult) = choice(player, options, hint);
            if (!isResult) continue;
            return Task.FromResult(result!);
        }
        
        throw new Exception($"No node choices left in queue (player: {player.LogName}, hint: {hint})");
    }

    public Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<string> ChooseString(Player player, string[] options, string hint)
    {
        while (StringChoices.Count > 0)
        {
            var choice = StringChoices.Dequeue();
            var (result, isResult) = choice(player, options, hint);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null string choice for {nameof(ChooseString)} (hint: {hint})");
            return Task.FromResult(result);
        }
        
        throw new Exception($"No string choices left in queue (player: {player.LogName}, hint: {hint})");
    }

    public Task<UMCore.Matches.Path> ChoosePath(Player player, UMCore.Matches.Path[] options, string hint)
    {
        while (PathChoices.Count > 0)
        {
            var choice = PathChoices.Dequeue();
            var (result, isResult) = choice(player, options, hint);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null path choice for {nameof(ChoosePath)} (hint: {hint})");
            return Task.FromResult(result);
        }
        
        throw new Exception($"No path choices left in queue (player: {player.LogName}, hint: {hint})");
    }

    public Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        while (TokenChoices.Count > 0)
        {
            var choice = TokenChoices.Dequeue();
            var (result, isResult) = choice(player, options, hint);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null token choice for {nameof(ChooseToken)} (hint: {hint})");
            return Task.FromResult(result);
        }
        
        throw new Exception($"No token choices left in queue (player: {player.LogName}, hint: {hint})");
    }

    public Task Setup(Player player, Match.SetupData setupData)
    {
        SetupCalled = true;
        return Task.CompletedTask;
    }

    public Task Update(Player player)
    {
        return Task.CompletedTask;
    }

    public void AssertAllChoiceQueuesEmpty()
    {
        CardChoices.Count.ShouldBe(0);
        FighterChoices.Count.ShouldBe(0);
        NodeChoices.Count.ShouldBe(0);       
    }
}