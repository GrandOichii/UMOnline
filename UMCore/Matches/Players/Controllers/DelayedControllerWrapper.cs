using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

public class DelayedControllerWrapper(
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
    ) : PlayerControllerWrapper(controller)
{
    public override async Task HandleActionChoice(string choice)
    {
        await Task.Delay(actionChoiceDelay);
    }

    public override async Task HandleAttackChoice(AvailableAttack choice)
    {
        await Task.Delay(attackChoiceDelay);
    }

    public override async Task HandleCardChoice(MatchCard choice)
    {
        await Task.Delay(cardChoiceDelay);
    }

    public override async Task HandleCardOrNothingChoice(MatchCard? choice)
    {
        await Task.Delay(cardOrNothingChoiceDelay);
    }

    public override async Task HandleFighterChoice(Fighter choice)
    {
        await Task.Delay(fighterChoiceDelay);
    }

    public override async Task HandleNodeChoice(MapNode choice)
    {
        await Task.Delay(nodeChoiceDelay);
    }

    public override async Task HandlePathChoice(Path choice)
    {
        await Task.Delay(pathChoiceDelay);
    }

    public override async Task HandlePlayerChoice(Player choice)
    {
        await Task.Delay(playerChoiceDelay);
    }

    public override async Task HandleStringChoice(string choice)
    {
        await Task.Delay(stringChoiceDelay);
    }

    public override async Task HandleTokenChoice(PlacedToken choice)
    {
        await Task.Delay(tokenChoiceDelay);
    }

    public override async Task HandleUpdate(Player player)
    {
        await Task.Delay(updateDelay);
    }
}