using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class FighterAsserts(Fighter fighter)
{
    public FighterAsserts HasName(string name)
    {
        fighter.Name.ShouldBe(name);
        return this;
    }

    public FighterAsserts DoesntHaveName(string name)
    {
        fighter.Name.ShouldNotBe(name);
        return this;
    }

    public FighterAsserts IsAlive()
    {
        fighter.IsAlive().ShouldBeTrue();
        return this;
    }

    public FighterAsserts IsDead()
    {
        fighter.IsAlive().ShouldBeFalse();
        return this;
    }

    public FighterAsserts HasHealth(int v)
    {
        fighter.Health.Current.ShouldBe(v);
        return this;
    }

    public FighterAsserts HasMaxHealth(int v)
    {
        fighter.Health.Max.ShouldBe(v);
        return this;
    }

    public FighterAsserts HasDamage(int v)
    {
        (fighter.Health.Max - fighter.Health.Current).ShouldBe(v);
        return this;
    }

    public FighterAsserts IsAtFullHealth()
    {
        fighter.Health.Current.ShouldBe(fighter.Health.Max);
        return this;
    }
}