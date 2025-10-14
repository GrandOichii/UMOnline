using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;

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
    public delegate (string, bool) PlayerAction(TestMatch match, Player player, string[] options);

    public Queue<PlayerAction> Actions { get; init; } = [];

    public bool SetupCalled { get; private set; } = false;

    public void AddEvent(Event e)
    {
        // TODO
    }

    public void AddLog(Log l)
    {
        // TODO
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        var match = (player.Match as TestMatch)!;
        var result = NEXT_ACTION;
        while (result == NEXT_ACTION)
        {
            if (!Actions.TryPeek(out var action))
                throw new Exception("No actions left in queue!");
            bool next;
            (result, next) = action(match, player, options);
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
        // TODO
        throw new NotImplementedException();
    }

    public Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<string> ChooseString(Player player, string[] options, string hint)
    {
        // TODO
        throw new NotImplementedException();
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
}