using System.Reflection.Metadata;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Tokens;

namespace UMCore.Matches.Players.Controllers;

public class RecorderPlayerController(
    IPlayerController controller
) : IPlayerController
{
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
        return result;
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var result = await controller.ChooseAttack(player, options);

        return result;
    }

    public async Task<MatchCard> ChooseCard(Player player, MatchCard[] options, string hint)
    {
        var result = await controller.ChooseCard(player, options, hint);

        return result;
    }

    public async Task<MatchCard?> ChooseCardOrNothing(Player player, MatchCard[] options, string hint)
    {
        var result = await controller.ChooseCardOrNothing(player, options, hint);

        return result;
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        var result = await controller.ChooseFighter(player, options, hint);

        return result;
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        var result = await controller.ChooseNode(player, options, hint);

        return result;
    }

    public async Task<Path> ChoosePath(Player player, Path[] options, string hint)
    {
        var result = await controller.ChoosePath(player, options, hint);

        return result;
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var result = await controller.ChoosePlayer(player, options, hint);

        return result;
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var result = await controller.ChooseString(player, options, hint);

        return result;
    }

    public async Task<PlacedToken> ChooseToken(Player player, PlacedToken[] options, string hint)
    {
        var result = await controller.ChooseToken(player, options, hint);

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