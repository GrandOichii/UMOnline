using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

public class PlayerControllerRecord
{
    public List<string> Actions { get; init; } = [];
    public List<string> AttackChoices { get; init; } = [];
    public List<string> CardChoices { get; init; } = [];
    public List<string> CardOrNothingChoices { get; init; } = [];
    public List<string> FighterChoices { get; init; } = [];
    public List<string> NodeChoices { get; init; } = [];
    public List<string> PathChoices { get; init; } = [];
    public List<string> PlayerChoices { get; init; } = [];
    public List<string> StringChoices { get; init; } = [];
    public List<string> TokenChoices { get; init; } = [];
}

public class RecorderControllerWrapper(
    IPlayerController controller
) : PlayerControllerWrapper(controller)
{
    public PlayerControllerRecord Record { get; } = new();

    public static string AttackChoiceToStr(AvailableAttack attack)
    {
        return $"{attack.Fighter.Id}_{attack.Target.Id}_{attack.AttackCard.Id}";
    }

    public static string CardChoiceToStr(MatchCard? card)
    {
        return card is null ? string.Empty : card.Id.ToString();
    }

    public static string FighterChoiceToStr(Fighter fighter)
    {
        return fighter.Id.ToString();
    }

    public override Task HandleActionChoice(string choice)
    {
        Record.Actions.Add(choice);
        return Task.CompletedTask;
    }

    public override Task HandleAttackChoice(AvailableAttack choice)
    {
        Record.AttackChoices.Add(AttackChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandleCardChoice(MatchCard choice)
    {
        Record.CardChoices.Add(CardChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandleCardOrNothingChoice(MatchCard? choice)
    {
        Record.CardOrNothingChoices.Add(CardChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandleFighterChoice(Fighter choice)
    {
        Record.FighterChoices.Add(FighterChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandleNodeChoice(MapNode choice)
    {
        Record.NodeChoices.Add(NodeChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandlePathChoice(Path choice)
    {
        Record.PathChoices.Add(PathChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandlePlayerChoice(Player choice)
    {
        Record.PlayerChoices.Add(PlayerChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public override Task HandleStringChoice(string choice)
    {
        Record.StringChoices.Add(choice);
        return Task.CompletedTask;
    }

    public override Task HandleTokenChoice(PlacedToken choice)
    {
        Record.TokenChoices.Add(TokenChoiceToStr(choice));
        return Task.CompletedTask;
    }

    public static string NodeChoiceToStr(MapNode node)
    {
        return node.Id.ToString();
    }

    public static string PathChoiceToStr(Path path)
    {
        return string.Join('_', path.Nodes.Select(n => n.Id));
    }

    public static string PlayerChoiceToStr(Player player)
    {
        return player.Idx.ToString();
    }

    public static string TokenChoiceToStr(PlacedToken token)
    {
        return token.Id.ToString();
    }
}