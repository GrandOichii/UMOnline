using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Effects;
using UMCore.Utility;

namespace UMCore.Matches;

public class CombatStepEffectsCollection
{
    public Dictionary<CombatStepTrigger, EffectCollection> Effects { get; }

    public CombatStepEffectsCollection(LuaTable data)
    {
        Effects = [];
        var combatStepEffectMappingRaw = LuaUtility.TableGet<LuaTable>(data, "CombatStepEffects");
        foreach (var keyRaw in combatStepEffectMappingRaw.Keys)
        {
            var key = (CombatStepTrigger)Convert.ToInt32(keyRaw);
            var table = combatStepEffectMappingRaw[keyRaw] as LuaTable;
            // TODO check for null
            var effects = new EffectCollection(table!);
            Effects.Add(key, effects);
        }
    }

    public int Count => Effects.Count;

    public async Task Execute(CombatStepTrigger trigger, Fighter by)
    {
        if (!Effects.TryGetValue(trigger, out var effects))
        {
            return;
        }

        effects.Execute(by, by.Owner);
    }
}