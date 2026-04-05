using NLua;
using UMCore.Matches.Attacks;
using UMCore.Utility;

namespace UMCore.Matches.Effects;

public class CombatStepEffectsCollection
{
    public Dictionary<CombatStepTrigger, EffectCollection> Effects { get; }

    public CombatStepEffectsCollection(Match match, LuaTable data)
    {
        Effects = [];
        var combatStepEffectMappingRaw = LuaUtility.TableGet<LuaTable>(data, "CombatStepEffects");
        foreach (var keyRaw in combatStepEffectMappingRaw.Keys)
        {
            var key = (CombatStepTrigger)Convert.ToInt32(keyRaw);
            var table = combatStepEffectMappingRaw[keyRaw] as LuaTable;
            var effects = new EffectCollection(match, table!);
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

        effects.Execute(new(by), new());
    }
}