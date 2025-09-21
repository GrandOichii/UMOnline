using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMCore.Utility;

namespace UMCore.Matches.Cards;

public class MatchCard
{
    public Player Owner { get; }
    public Card Card { get; }
    public int Idx { get; }

    public EffectCollection SchemeEffect { get; }

    public string LogName => $"({Idx}){Card.Template.Name}[{Owner.Idx}]";

    public MatchCard(Player owner, Card card)
    {
        Owner = owner;
        Card = card;

        var match = owner.Match;
        Idx = match.AddCard(this);

        // effects
        LuaTable data;
        try
        {
            match.LState.DoString(card.Template.Script);
            var creationFunc = LuaUtility.GetGlobalF(match.LState, "_Create");
            var returned = creationFunc.Call();
            data = LuaUtility.GetReturnAs<LuaTable>(returned);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to run card creation function in card {Card.Template.Name}", e); // TODO type
        }

        try
        {
            SchemeEffect = new(LuaUtility.TableGet<LuaTable>(data, "SchemeEffects"));
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get scheme effects for card {card.Template.Name}", e); // TODO type
        }

        // TODO scheme text

        // TODO immediately, during combat and after combat
    }

    public int GetBoostValue()
    {
        return Card.Template.Boost;
    }    

    public bool CanBePlayedAsScheme(Fighter fighter)
    {
        return Card.Template.Type == "Scheme" && Card.CanBePlayedBy(fighter.Name);
    }

    public IEnumerable<Fighter> GetCanBePlayedBy()
    {
        return Owner.GetAliveFighters().Where(CanBePlayedAsScheme);
    }

    public async Task ExecuteEffects(Fighter by)
    {
        Owner.Match.Logger?.LogDebug("Executing scheme effects of card {CardLogName} by fighter {FighterLogName}", LogName, by.LogName);

        SchemeEffect.Execute(LuaUtility.CreateTable(Owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", by }
        }));
    }
}