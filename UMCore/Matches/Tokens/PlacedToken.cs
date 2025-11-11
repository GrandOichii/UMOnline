using UMCore.Matches.Effects;

namespace UMCore.Matches.Tokens;

public class PlacedToken
{
    public Token Original { get; }
    public MapNode Node { get; private set; }

    public PlacedToken(Token original, MapNode node)
    {
        Original = original;
        Node = node;
    }
    
    public string GetName()
    {
        return Original.Name;
    }

    public IEnumerable<EffectCollection> GetOnStepEffects(Fighter fighter)
    {
        return Original
            .OnStepEffects.Where(e => e.ConditionsMet(new(Original.Originator), new(fighter)));
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
            effect.Execute(new(Original.Originator, this), new()); // TODO? set fighter to null
        }
        await Node.Parent.Match.UpdateClients();
    }

    public async Task MoveTo(MapNode node)
    {
        Node.Tokens.Remove(this);
        node.Tokens.Add(this);
        Node = node;
    }
}