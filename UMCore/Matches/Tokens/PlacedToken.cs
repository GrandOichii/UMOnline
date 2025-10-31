namespace UMCore.Matches.Tokens;

public class PlacedToken
{
    public Token Original { get; }

    public PlacedToken(Token original)
    {
        Original = original;
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
}