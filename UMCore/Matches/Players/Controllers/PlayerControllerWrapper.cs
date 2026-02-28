using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;


public abstract class PlayerControllerWrapper(
    IPlayerController controller
) : IPlayerController
{
    public abstract Task HandleActionChoice(string choice);
    public abstract Task HandleAttackChoice(AvailableAttack choice);
    public abstract Task HandleCardChoice(MatchCard choice);
    public abstract Task HandleCardOrNothingChoice(MatchCard? choice);
    public abstract Task HandleFighterChoice(Fighter choice);
    public abstract Task HandleNodeChoice(MapNode choice);
    public abstract Task HandlePathChoice(Path choice);
    public abstract Task HandlePlayerChoice(Player choice);
    public abstract Task HandleTokenChoice(PlacedToken choice);
    public abstract Task HandleStringChoice(string choice);

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
        await HandleActionChoice(result);
        return result;
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var result = await controller.ChooseAttack(player, options);
        await HandleAttackChoice(result);
        return result;
    }

    public async Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        var result = await controller.ChooseCard(player, options, hint);
        await HandleCardChoice(result);

        return result;
    }

    public async Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        var result = await controller.ChooseCardOrNothing(player, options, hint);
        await HandleCardOrNothingChoice(result);

        return result;
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var result = await controller.ChooseFighter(player, options, hint);
        await HandleFighterChoice(result);

        return result;
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var result = await controller.ChooseNode(player, options, hint);
        await HandleNodeChoice(result);

        return result;
    }

    public async Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        var result = await controller.ChoosePath(player, options, hint);
        await HandlePathChoice(result);

        return result;
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var result = await controller.ChoosePlayer(player, options, hint);
        await HandlePlayerChoice(result);

        return result;
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var result = await controller.ChooseString(player, options, hint);
        await HandleStringChoice(result);

        return result;
    }

    public async Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        var result = await controller.ChooseToken(player, options, hint);
        await HandleTokenChoice(result);

        return result;
    }

    public async Task Setup(Player player, Match.SetupData setupData)
    {
        await HandleSetup(player, setupData);
        await controller.Setup(player, setupData);
    }

    public virtual Task HandleSetup(Player player, Match.SetupData setupData)
    {
        return Task.CompletedTask;
    }

    public async Task Update(Player player)
    {
        await HandleUpdate(player);
        await controller.Update(player);
    }

    public virtual Task HandleUpdate(Player player)
    {
        return Task.CompletedTask;
    }
}