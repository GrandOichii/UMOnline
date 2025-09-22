namespace UMCore.Matches.Players;

public enum TurnPhaseTrigger
{
    START = 0,
    END = 1
}

public interface ITurnPhase
{

    public Task Execute(Player player);
}

public abstract class TriggerringTurnPhase : ITurnPhase
{
    abstract public TurnPhaseTrigger GetTrigger();

    public async Task Execute(Player player)
    {
        await Exec(player);
        
        var phase = GetTrigger();
        await player.EmitTurnPhaseTrigger(phase);
    }

    public abstract Task Exec(Player player);
}

public class StartPhase : TriggerringTurnPhase
{
    public override async Task Exec(Player player)
    {
        await player.StartTurn();
    }

    public override TurnPhaseTrigger GetTrigger() => TurnPhaseTrigger.START;
}


public class EndPhase : TriggerringTurnPhase
{
    public override async Task Exec(Player player)
    {
        await player.EndTurn();
    }

    public override TurnPhaseTrigger GetTrigger() => TurnPhaseTrigger.END;
}

public class ActionsPhase : ITurnPhase
{
    public async Task Execute(Player player)
    {
        await player.TakeActions();
    }
}