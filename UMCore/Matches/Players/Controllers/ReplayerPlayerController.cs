

using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

public class ReplayerPlayerController(
    PlayerControllerRecord record
) : IPlayerController
{
    private readonly Stack<string> _actionQueue = new(record.Actions);
    private readonly Stack<string> _attackQueue = new(record.AttackChoices);
    private readonly Stack<string> _cardQueue = new(record.CardChoices);
    private readonly Stack<string> _cardOrNothingQueue = new(record.CardOrNothingChoices);
    private readonly Stack<string> _fighterQueue = new(record.FighterChoices);
    private readonly Stack<string> _nodeQueue = new(record.NodeChoices);
    private readonly Stack<string> _pathQueue = new(record.PathChoices);
    private readonly Stack<string> _playerQueue = new(record.PlayerChoices);
    private readonly Stack<string> _stringQueue = new(record.StringChoices);
    private readonly Stack<string> _tokenQueue = new(record.TokenChoices);
    
    public void AddEvent(Event e)
    {
    }

    public void AddLog(Log l)
    {
    }

    public Task<string> ChooseAction(Player player, string[] options)
    {
        return Task.FromResult(
            _actionQueue.Pop()
        );
    }

    public Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        throw new NotImplementedException();
    }

    public Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        var id = int.Parse(_cardQueue.Pop());
        return Task.FromResult(
            options.Single(c => c.Id == id)
        );
    }

    public Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        var id = _cardOrNothingQueue.Pop();
        MatchCard? result = id switch
        {
            "" => null,
            _ => options.Single(c => c.Id == int.Parse(id))
        };
        return Task.FromResult(result);
    }

    public Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var id = int.Parse(_fighterQueue.Pop());
        return Task.FromResult(
            options.Single(c => c.Id == id)
        );
    }

    public Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var id = int.Parse(_nodeQueue.Pop());
        return Task.FromResult(
            options.Single(c => c.Id == id)
        );
    }

    public Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        throw new NotImplementedException();

        // var ids = _pathQueue.Pop().Split('_').Select(int.Parse);
        // return Task.FromResult(
        //     options.First()
        // );
    }

    public Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var idx = int.Parse(_playerQueue.Pop());
        return Task.FromResult(
            options.Single(c => c.Idx == idx)
        );
    }

    public Task<string> ChooseString(Player player, string[] options, string hint)
    {
        return Task.FromResult(
            _stringQueue.Pop()
        );
    }

    public Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        var id = int.Parse(_tokenQueue.Pop());
        return Task.FromResult(
            options.Single(c => c.Id == id)
        );
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