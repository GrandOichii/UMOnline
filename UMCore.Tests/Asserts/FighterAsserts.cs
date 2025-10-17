using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class FighterAsserts(Fighter fighter)
{
    public FighterAsserts IsAlive()
    {
        fighter.IsAlive().ShouldBeTrue();
        return this;
    }

    public FighterAsserts HasHealth(int v)
    {
        fighter.Health.Current.ShouldBe(v);
        return this;
    }
}