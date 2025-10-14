using Shouldly;

namespace UMCore.Tests.Asserts;

public class MatchAsserts(TestMatchWrapper match)
{
    public MatchAsserts PlayerCount(int count)
    {
        match.Match.Players.Count.ShouldBe(count);
        return this;
    }

    public MatchAsserts CantRun()
    {
        match.Match.CanRun().ShouldBeFalse();
        return this;
    }

    public MatchAsserts CanRun()
    {
        match.Match.CanRun().ShouldBeTrue();
        return this;
    }

    public MatchAsserts Threw()
    {
        match.Exception.ShouldNotBeNull();
        return this;
    }

    public MatchAsserts DidntThrow()
    {
        match.Exception.ShouldBeNull();
        return this;
    }
}