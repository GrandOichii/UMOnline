using System.Buffers;
using UMCore.Matches.Effects;

namespace UMCore.Matches.Tokens;

public class PlacedToken
{
    public int Id { get; }
    public Token Original { get; }
    public MapNode Node { get; private set; }

    public PlacedToken(int id, Token original, MapNode node)
    {
        Id = id;
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
        Original.Originator.Match.Logger?.LogDebug("Token '{TokeName}' is removed from node with id = {NodeId}", Original.Name, Node.Id);
        Original.Originator.Match.Logs.Public($"A {Original.Name} token was returned to the box");
        await Node.Parent.Match.UpdateClients();

        await ResolveWhenReturnedToBoxEffects();
    }

    public async Task ResolveWhenReturnedToBoxEffects()
    {
        var effects = Original.WhenReturnedToBoxEffects;

        // TODO order effects

        foreach (var effect in effects)
        {
            effect.Execute(new(Original.Originator, this), new());
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