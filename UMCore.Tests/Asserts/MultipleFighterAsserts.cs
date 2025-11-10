using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class MultipleFighterAsserts(List<Fighter> fighters)
{
    private readonly List<FighterAsserts> _fighters = [.. fighters.Select(f => new FighterAsserts(f))];

    public MultipleFighterAsserts AreAtFullHealth()
    {
        foreach (var f in _fighters)
            f.IsAtFullHealth();
        return this;
    }

    public MultipleFighterAsserts HaveDamage(int amount)
    {
        foreach (var f in _fighters)
            f.HasDamage(amount);
        return this;
    }
}