using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMCore.Utility;

namespace UMCore.Matches.Cards;

public class MatchCard
{
    public Player Owner { get; }
    public CardTemplate Card { get; }
    public int Idx { get; }

    public EffectCollection SchemeEffect { get; }
    public Dictionary<CombatStepTrigger, EffectCollection> CombatStepEffects { get; }

    public string LogName => $"({Idx}){Card.Template.Name}[{Owner.Idx}]";

    public MatchCard(Player owner, CardTemplate card)
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
            SchemeEffect = new(LuaUtility.TableGet<LuaTable>(data, "Scheme"));
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get scheme effects for card {card.Template.Name}", e); // TODO type
        }

        CombatStepEffects = [];
        try
        {
            var combatStepEffectMappingRaw = LuaUtility.TableGet<LuaTable>(data, "CombatStepEffects");
            foreach (var keyRaw in combatStepEffectMappingRaw.Keys)
            {
                var key = (CombatStepTrigger)Convert.ToInt32(keyRaw);
                var table = combatStepEffectMappingRaw[keyRaw] as LuaTable;
                // TODO check for null
                var effects = new EffectCollection(table!);
                CombatStepEffects.Add(key, effects);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get combat step effects for card {card.Template.Name}", e); // TODO type
        }
    }

    public int GetBoostValue()
    {
        return Card.Template.Boost;
    }

    public bool CanBePlayedAsScheme(Fighter fighter)
    {
        return Card.Template.Type == "Scheme" && Card.CanBePlayedBy(fighter.GetName());
    }

    public bool CanBeUsedAsAttack(Fighter fighter)
    {
        // TODO some effects change this
        return (Card.Template.Type == "Attack" || Card.Template.Type == "Versatile") && Card.CanBePlayedBy(fighter.GetName());
    }

    public bool CanBeUsedAsDefence(Fighter fighter)
    {
        // TODO some effects change this
        return (Card.Template.Type == "Defence" || Card.Template.Type == "Versatile") && Card.CanBePlayedBy(fighter.GetName());
    }

    public IEnumerable<Fighter> GetCanBePlayedBy()
    {
        return Owner.GetAliveFighters().Where(CanBePlayedAsScheme);
    }

    public async Task ExecuteSchemeEffects(Fighter by)
    {
        Owner.Match.Logger?.LogDebug("Executing scheme effects of card {CardLogName} by fighter {FighterLogName}", LogName, by.LogName);

        SchemeEffect.Execute(LuaUtility.CreateTable(Owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", by },
            { "owner", by.Owner },
        }));

        // TODO update clients about played scheme card
    }

    public async Task PlaceIntoDiscard()
    {
        await Owner.DiscardPile.Add([this]);
    }

    public async Task ExecuteCombatStepTrigger(CombatStepTrigger trigger, Fighter by)
    {
        // TODO
        if (!CombatStepEffects.TryGetValue(trigger, out var effects))
        {
            return;
        }

        effects.Execute(LuaUtility.CreateTable(Owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", by },
            { "owner", by.Owner },
        }));
    }
}