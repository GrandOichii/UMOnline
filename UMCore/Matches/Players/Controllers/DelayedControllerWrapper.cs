using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

public class DelayedControllerWrapper : PlayerControllerWrapper
{
    private readonly TimeSpan _updateDelay;
    private readonly TimeSpan _actionChoiceDelay;
    private readonly TimeSpan _attackChoiceDelay;
    private readonly TimeSpan _cardChoiceDelay;
    private readonly TimeSpan _cardOrNothingChoiceDelay;
    private readonly TimeSpan _fighterChoiceDelay;
    private readonly TimeSpan _nodeChoiceDelay;
    private readonly TimeSpan _pathChoiceDelay;
    private readonly TimeSpan _playerChoiceDelay;
    private readonly TimeSpan _stringChoiceDelay;
    private readonly TimeSpan _tokenChoiceDelay;    
    
    public DelayedControllerWrapper(
        IPlayerController controller,
        TimeSpan updateDelay,
        TimeSpan actionChoiceDelay,
        TimeSpan attackChoiceDelay,
        TimeSpan cardChoiceDelay,
        TimeSpan cardOrNothingChoiceDelay,
        TimeSpan fighterChoiceDelay,
        TimeSpan nodeChoiceDelay,
        TimeSpan pathChoiceDelay,
        TimeSpan playerChoiceDelay,
        TimeSpan stringChoiceDelay,
        TimeSpan tokenChoiceDelay
    ) : base(controller)
    {
        _updateDelay = updateDelay;
        _actionChoiceDelay = actionChoiceDelay;
        _attackChoiceDelay = attackChoiceDelay;
        _cardChoiceDelay = cardChoiceDelay;
        _cardOrNothingChoiceDelay = cardOrNothingChoiceDelay;
        _fighterChoiceDelay = fighterChoiceDelay;
        _nodeChoiceDelay = nodeChoiceDelay;
        _pathChoiceDelay = pathChoiceDelay;
        _playerChoiceDelay = playerChoiceDelay;
        _stringChoiceDelay = stringChoiceDelay;
        _tokenChoiceDelay = tokenChoiceDelay;        
    }

    public DelayedControllerWrapper(
        IPlayerController controller,
        TimeSpan delay
    ) : this(
        controller,
        delay,
        delay,
        delay,
        delay,
        delay,
        delay,
        delay,
        delay,
        delay,
        delay,
        delay
    ) {}
    
    public override async Task HandleActionChoice(string choice)
    {
        await Task.Delay(_actionChoiceDelay);
    }

    public override async Task HandleAttackChoice(AvailableAttack choice)
    {
        await Task.Delay(_attackChoiceDelay);
    }

    public override async Task HandleCardChoice(MatchCard choice)
    {
        await Task.Delay(_cardChoiceDelay);
    }

    public override async Task HandleCardOrNothingChoice(MatchCard? choice)
    {
        await Task.Delay(_cardOrNothingChoiceDelay);
    }

    public override async Task HandleFighterChoice(Fighter choice)
    {
        await Task.Delay(_fighterChoiceDelay);
    }

    public override async Task HandleNodeChoice(MapNode choice)
    {
        await Task.Delay(_nodeChoiceDelay);
    }

    public override async Task HandlePathChoice(Path choice)
    {
        await Task.Delay(_pathChoiceDelay);
    }

    public override async Task HandlePlayerChoice(Player choice)
    {
        await Task.Delay(_playerChoiceDelay);
    }

    public override async Task HandleStringChoice(string choice)
    {
        await Task.Delay(_stringChoiceDelay);
    }

    public override async Task HandleTokenChoice(PlacedToken choice)
    {
        await Task.Delay(_tokenChoiceDelay);
    }

    public override async Task HandleUpdate(Player player)
    {
        await Task.Delay(_updateDelay);
    }
}