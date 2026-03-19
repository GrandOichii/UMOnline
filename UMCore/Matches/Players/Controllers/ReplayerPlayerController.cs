

using System.Diagnostics;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

// public class ReplayerPlayerController(
//     PlayerControllerRecord record
// ) : IPlayerController
public class ReplayerPlayerController : IPlayerController
{
    private readonly Stack<string> _actionQueue;
    private readonly Stack<string> _attackQueue;
    private readonly Stack<string> _cardQueue;
    private readonly Stack<string> _cardOrNothingQueue;
    private readonly Stack<string> _fighterQueue;
    private readonly Stack<string> _nodeQueue;
    private readonly Stack<string> _pathQueue;
    private readonly Stack<string> _playerQueue;
    private readonly Stack<string> _stringQueue;
    private readonly Stack<string> _tokenQueue;

    public ReplayerPlayerController(PlayerControllerRecord record)
    {
        _actionQueue = new(record.Actions.AsEnumerable().Reverse());
        _attackQueue = new(record.AttackChoices.AsEnumerable().Reverse());
        _cardQueue = new(record.CardChoices.AsEnumerable().Reverse());
        _cardOrNothingQueue = new(record.CardOrNothingChoices.AsEnumerable().Reverse());
        _fighterQueue = new(record.FighterChoices.AsEnumerable().Reverse());
        _nodeQueue = new(record.NodeChoices.AsEnumerable().Reverse());
        _pathQueue = new(record.PathChoices.AsEnumerable().Reverse());
        _playerQueue = new(record.PlayerChoices.AsEnumerable().Reverse());
        _stringQueue = new(record.StringChoices.AsEnumerable().Reverse());
        _tokenQueue = new(record.TokenChoices.AsEnumerable().Reverse());
    }

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
        var target = _attackQueue.Pop();
        var split = target.Split("_");
        // Console.WriteLine(target);
        // foreach (var o in options)
        // {
        //     Console.WriteLine($"\t{RecorderControllerWrapper.AttackChoiceToStr(o)}");
        // }

        return Task.FromResult(
            options.Single(o => 
                o.Fighter.Id == int.Parse(split[0]) &&
                o.Target.Id == int.Parse(split[1]) &&
                o.AttackCard.Id == int.Parse(split[2])
            )
        );
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
        var target = _pathQueue.Pop();
        // Debug.Print(target);
        // foreach (var o in options)
        // {
        //     Debug.Print($"\t{RecorderControllerWrapper.PathChoiceToStr(o)}");
        // }
        return Task.FromResult(
            options.Single(o => RecorderControllerWrapper.PathChoiceToStr(o) == target)
        );
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