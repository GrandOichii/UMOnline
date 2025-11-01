namespace UMCore.Matches.Tokens;

public class PlacedToken
{
    public Token Original { get; }
    public MapNode Node { get; }

    public PlacedToken(Token original, MapNode node)
    {
        Original = original;
        Node = node;
    }
    
    public string GetName()
    {
        return Original.Name;
    }

    public IEnumerable<FighterPredicateEffect> GetOnStepEffects(Fighter fighter)
    {
        return Original
            .OnStepEffects.Where(e => e.Accepts(fighter));
    }

    public async Task Remove()
    {
        Node.Tokens.Remove(this);
        // TODO log
        // TODO public log
        await Node.Parent.Match.UpdateClients();

        await ResolveWhenReturnedToBoxEffects();
    }

    public async Task ResolveWhenReturnedToBoxEffects()
    {
        var effects = Original.WhenReturnedToBoxEffects;

        // TODO order effects

        foreach (var effect in effects)
        {
            effect.Execute(Original.Originator, Original.Originator.Owner, this); // TODO? set fighter to null
        }
        await Node.Parent.Match.UpdateClients();
    }
}