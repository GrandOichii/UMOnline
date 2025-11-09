namespace UMCore.Matches.Players;

public interface IAction
{
    public string Name();
    public Task Execute(Player player);
    public bool CanBeTaken(Player player);
}

public class ManoeuvreAction : IAction
{
    public string Name() => "Manoeuvre";

    public async Task Execute(Player player)
    {
        var drawAmount = player.Match.Config.ManoeuvreDrawAmount;
        var mods = player.Match.GetAliveFighters().SelectMany(f => f.ManoeuvreDrawAmountModifiers);
        foreach (var mod in mods)
        {
            drawAmount = mod.Modify(player, drawAmount);
        }
        await player.Hand.Draw(drawAmount);

        await player.MoveFighters(isManoeuvre: true, canMoveOverFriendly: true);
    }

    public bool CanBeTaken(Player player)
    {
        return true;
    }
}

public class AttackAction : IAction
{
    public string Name() => "Attack";

    public async Task Execute(Player player)
    {
        var available = player.GetAvailableAttacks().ToList();
        var attack = await player.Controller.ChooseAttack(player, [.. available]);
        await player.Match.ProcessAttack(player, attack);
    }

    public bool CanBeTaken(Player player)
    {
        return player.GetAvailableAttacks().Any();
    }
}

public class SchemeAction : IAction
{
    public string Name() => "Scheme";

    public bool CanBeTaken(Player player)
    {
        return player.Hand.GetPlayableSchemeCards().Any();
    }

    public async Task Execute(Player player)
    {
        var options = player.Hand.GetPlayableSchemeCards();

        var chosen = await player.Controller.ChooseCardInHand(player, player.Idx, [.. options], $"Choose a scheme card to play");
        var availableFighters = chosen.GetCanBePlayedBy().ToList();
        if (availableFighters.Count == 0)
        {
            throw new MatchException($"Player {player.LogName} chose {chosen.LogName} to play as scheme card, when no fighter of theirs can play it");
        }
        var fighter = availableFighters[0];
        if (availableFighters.Count > 1)
        {
            fighter = await player.Controller.ChooseFighter(player, [.. availableFighters], $"Choose a fighter to play {chosen.LogName}");
        }

        await player.PlayScheme(chosen, fighter);
    }
}