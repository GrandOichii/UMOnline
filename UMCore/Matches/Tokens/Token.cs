using NLua;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Utility;

namespace UMCore.Matches.Tokens;

public class Token
{
    public string Name { get; }
    public int Amount { get; private set; }
    public List<EffectCollection> WhenReturnedToBoxEffects { get; }
    public List<FighterPredicateEffect> OnStepEffects { get; }
    public Fighter Originator { get; }

    public Token(string name, Fighter fighter, LuaTable data)
    {
        Name = name;
        Originator = fighter;
        Amount = LuaUtility.GetInt(data, "Amount");

        WhenReturnedToBoxEffects = [];
        try
        {
            var onAttackEffects = LuaUtility.TableGet<LuaTable>(data, "WhenReturnedToBox");
            foreach (var value in onAttackEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                WhenReturnedToBoxEffects.Add(new(table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get WhenReturnedToBox effects for token {name}", e);
        }

        OnStepEffects = [];
        try
        {
            var onStepEffects = LuaUtility.TableGet<LuaTable>(data, "OnStepEffects");
            foreach (var value in onStepEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                OnStepEffects.Add(new(fighter, table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get on step effects for token {name}", e);
        }
    }
    
    public PlacedToken? GetPlacedToken()
    {
        if (Amount == 0)
        {
            return null;
        }
        --Amount;
        return new(this);
    }

    public void SetAmount(int v)
    {
        Amount = v;
    }
}