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
        await player.Draw(1); // TODO move to configuration

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
        return false;
        // TODO check whether a scheme card is in the player's hand AND can be played by alive fighters
        // throw new NotImplementedException();
        
    }

    public Task Execute(Player player)
    {
        // Choose a scheme card and play it
        // TODO
        throw new NotImplementedException();
    }
}