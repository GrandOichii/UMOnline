using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Tests.Setup;

public class TestPlayerController : IPlayerController
{
    public void AddEvent(Event e)
    {
        // TODO
        throw new NotImplementedException();
    }

    public void AddLog(Log l)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<string> ChooseAction(Player player, string[] options)
    {
        // TODO
        throw new NotImplementedException();
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
        // TODO
        throw new NotImplementedException();
    }

    public Task Update(Player player)
    {
        // TODO
        throw new NotImplementedException();
    }
}