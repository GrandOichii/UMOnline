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

public class RecorderPlayerController(
    IPlayerController controller
) : IPlayerController
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

    public void AddEvent(Event e)
    {
        controller.AddEvent(e);
    }

    public void AddLog(Log l)
    {
        controller.AddLog(l);
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        var result = await controller.ChooseAction(player, options);
        Record.Actions.Add(result);
        return result;
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var result = await controller.ChooseAttack(player, options);
        Record.AttackChoices.Add(AttackChoiceToStr(result));
        return result;
    }

    public async Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        var result = await controller.ChooseCard(player, options, hint);
        Record.CardChoices.Add(CardChoiceToStr(result));

        return result;
    }

    public async Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        var result = await controller.ChooseCardOrNothing(player, options, hint);
        Record.CardChoices.Add(CardChoiceToStr(result));

        return result;
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var result = await controller.ChooseFighter(player, options, hint);
        Record.FighterChoices.Add(FighterChoiceToStr(result));

        return result;
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var result = await controller.ChooseNode(player, options, hint);
        Record.NodeChoices.Add(NodeChoiceToStr(result));

        return result;
    }

    public async Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        var result = await controller.ChoosePath(player, options, hint);
        Record.PathChoices.Add(PathChoiceToStr(result));

        return result;
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var result = await controller.ChoosePlayer(player, options, hint);
        Record.PlayerChoices.Add(PlayerChoiceToStr(result));

        return result;
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var result = await controller.ChooseString(player, options, hint);
        Record.StringChoices.Add(result);

        return result;
    }

    public async Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        var result = await controller.ChooseToken(player, options, hint);
        Record.TokenChoices.Add(TokenChoiceToStr(result));

        return result;
    }

    public Task Setup(Player player, Match.SetupData setupData)
    {
        return controller.Setup(player, setupData);
    }

    public Task Update(Player player)
    {
        return controller.Update(player);
    }
}