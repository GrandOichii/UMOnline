using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;


public class PlaybackPlayerController : IPlayerController
{
    public void AddEvent(Event e)
    {
        throw new NotImplementedException();
    }

    public void AddLog(Log l)
    {
        throw new NotImplementedException();
    }

    public Task<string> ChooseAction(Player player, string[] options)
    {
        throw new NotImplementedException();
    }

    public Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        throw new NotImplementedException();
    }

    public Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<string> ChooseString(Player player, string[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task Setup(Player player, Match.SetupData setupData)
    {
        throw new NotImplementedException();
    }

    public Task Update(Player player)
    {
        throw new NotImplementedException();
    }
}