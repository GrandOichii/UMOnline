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
        await player.Hand.Draw(1); // TODO move to configuration

        await player.MoveFighters(allowBoost: true, canMoveOverFriendly: true);
    }

    public bool CanBeTaken(Player player)
    {
        return true;
    }
}

public class FightAction : IAction
{
    public string Name() => "Fight";

    public Task Execute(Player player)
    {
        // Play a card and initiate combat
        // TODO
        throw new NotImplementedException();
    }

    public bool CanBeTaken(Player player)
    {
        // TODO
        return false;
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

        var chosen = await player.Controller.ChooseCardInHand(player, player.Idx, options, $"Choose a scheme card to play");
        var availableFighters = chosen.GetCanBePlayedBy().ToList();
        if (availableFighters.Count == 0)
        {
            throw new Exception($"Player {player.LogName} chose {chosen.LogName} to play as scheme card, when no fighter of theirs can play it"); // TODO type
        }
        var fighter = availableFighters[0];
        if (availableFighters.Count > 1)
        {
            fighter = await player.Controller.ChooseFighter(player, availableFighters, $"Choose a fighter to play {chosen.LogName}");
        }

        await player.PlayScheme(chosen, fighter);
    }
}