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

    public Task Execute(Player player)
    {
        var options = player.Hand.GetPlayableSchemeCards();

        // TODO pick option
        // TODO if card can be played by multiple fighters, choose which fighter plays the card
        // TODO resolve the card's effects
    }
}