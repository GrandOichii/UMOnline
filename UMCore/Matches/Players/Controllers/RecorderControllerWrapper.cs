using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

public class PlayerControllerRecord
{
    public List<string> Actions { get; } = [];
    public List<string> AttackChoices { get; } = [];
    public List<string> CardChoices { get; } = [];
    public List<string> CardOrNothingChoices { get; } = [];
    public List<string> FighterChoices { get; } = [];
    public List<string> NodeChoices { get; } = [];
    public List<string> PathChoices { get; } = [];
    public List<string> PlayerChoices { get; } = [];
    public List<string> StringChoices { get; } = [];
    public List<string> TokenChoices { get; } = [];
}

public class RecorderControllerWrapper(
    IPlayerController controller
) : PlayerControllerWrapper(controller)
{
    public PlayerControllerRecord Record { get; } = new();

    public string AttackChoiceToStr(AvailableAttack attack)
    {
        return $"{attack.Fighter.Id}_{attack.Target.Id}_{attack.AttackCard.Id}";
    }

    public string CardChoiceToStr(MatchCard? card)
    {
        return card is null ? string.Empty : card.Id.ToString();
    }

    public string FighterChoiceToStr(Fighter fighter)
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

    public override async Task HandleCardChoice(MatchCard choice)
    {
        await HandleCardOrNothingChoice(choice);
    }

    public override Task HandleCardOrNothingChoice(MatchCard? choice)
    {
        Record.CardChoices.Add(CardChoiceToStr(choice));
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

    public string NodeChoiceToStr(MapNode node)
    {
        return node.Id.ToString();
    }

    public string PathChoiceToStr(Path path)
    {
        return string.Join('_', path.Nodes.Select(n => n.Id));
    }

    public string PlayerChoiceToStr(Player player)
    {
        return player.Idx.ToString();
    }

    public string TokenChoiceToStr(PlacedToken token)
    {
        return token.Id.ToString();
    }


}