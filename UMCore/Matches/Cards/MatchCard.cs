using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMCore.Utility;

namespace UMCore.Matches.Cards;

public class MatchCard : IHasData<MatchCard.Data>
{
    public Player Owner { get; }
    public CardTemplate Template { get; }
    public int Id { get; }

    public EffectCollection SchemeEffect { get; }
    public List<SchemeRequirement> SchemeRequirements { get; }
    public CombatStepEffectsCollection CombatStepEffects { get; }

    public string LogName => $"({Id}){Template.Key}[{Owner.Idx}]";

    public string FormattedLogName => $"{{{Template.Key}:{Id}:{Template.Name}}}";

    public MatchCard(Player owner, CardTemplate card)
    {
        Owner = owner;
        Template = card;

        var match = owner.Match;
        Id = match.AddCard(this);

        // effects
        LuaTable data;
        try
        {
            match.LState.DoString(card.Script);
            var creationFunc = LuaUtility.GetGlobalF(match.LState, "_Create");
            var returned = creationFunc.Call();
            data = LuaUtility.GetReturnAs<LuaTable>(returned);
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to run card creation function in card {Template.Name}", e);
        }

        try
        {
            SchemeEffect = new(LuaUtility.TableGet<LuaTable>(data, "Scheme"));
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get scheme effects for card {card.Name}", e);
        }

        try
        {
            CombatStepEffects = new(data);
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get combat step effects for card {card.Name}", e);
        }

        SchemeRequirements = [];
        try
        {
            var schemeRequirements = LuaUtility.TableGet<LuaTable>(data, "SchemeRequirements");
            foreach (var value in schemeRequirements.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                var requirement = new SchemeRequirement(table!);
                SchemeRequirements.Add(requirement);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get scheme requirements for card {card.Name}", e);
        }

    }

    public bool HasEffects()
    {
        return CombatStepEffects.Count > 0;
    }

    public int? GetBoostValue()
    {
        return Template.Boost;
    }

    public bool CanBePlayedAsScheme(Fighter fighter)
    {
        if (Template.Type != "Scheme") return false;
        if (!Template.CanBePlayedBy(fighter.GetName())) return false;

        return SchemeRequirements.All(req => req.ConditionsMet(fighter));
    }

    public bool CanBeUsedAsAttack(Fighter fighter)
    {
        // TODO some effects change this
        return (Template.Type == "Attack" || Template.Type == "Versatile") &&
                Template.CanBePlayedBy(fighter.GetName());
    }

    public bool CanBeUsedAsDefence(Fighter fighter)
    {
        // TODO some effects change this
        return (Template.Type == "Defence" || Template.Type == "Defense" || Template.Type == "Versatile") &&
                Template.CanBePlayedBy(fighter.GetName());
    }

    public bool CanBeCancelled(Player byPlayer)
    {
        foreach (var fighter in Owner.Match.Fighters)
        {
            foreach (var forbid in fighter.CardCancellingForbids)
            {
                if (forbid.Forbids(this, byPlayer))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public IEnumerable<Fighter> GetCanBePlayedBy()
    {
        return Owner.GetAliveFighters().Where(CanBePlayedAsScheme);
    }

    public async Task ExecuteSchemeEffects(Fighter by)
    {
        Owner.Match.Logger?.LogDebug("Executing scheme effects of card {CardLogName} by fighter {FighterLogName}", LogName, by.LogName);
        Owner.Match.Logs.Public($"Player {by.Owner.FormattedLogName} played Scheme card {FormattedLogName}");
        SchemeEffect.Execute(new(by), new());

        await Owner.Match.UpdateClients();        
    }

    public async Task PlaceIntoDiscard()
    {
        await Owner.DiscardPile.Add([this]);
    }

    public bool HasLabel(string label)
    {
        return Template.Labels.Contains(label);
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Id = Id,
            Key = Template.Key
        };
    }

    public class Data
    { 
        public required string Key { get; init; }
        public required int Id { get; init; }
    }
}