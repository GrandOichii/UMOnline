using NLua;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;

namespace UMCore.Matches.Fighters;

public class CombatResolutionEffect(Fighter fighter, LuaTable data)
{
    private readonly Fighter _fighter = fighter;
    private readonly EffectCollection _effects = new(data);

    public void Execute(Player player)
    {
        _effects.Execute(_fighter, new(player));
    }
}