using System.Runtime.ExceptionServices;
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

    public MatchAsserts CrashedIntentionally()
    {
        match.Exception.ShouldNotBeNull();
        if (match.Exception.GetType() != typeof(IntentionalCrashException))
        {
            ExceptionDispatchInfo.Capture(match.Exception).Throw();
        }
        match.Exception.ShouldBeOfType<IntentionalCrashException>();

        return this;
    }

    public MatchAsserts CrashedUnintentionally()
    {
        match.Exception.ShouldNotBeNull();
        match.Exception.ShouldNotBeOfType<IntentionalCrashException>();
        return this;
    }

    public MatchAsserts DidntThrow()
    {
        match.Exception.ShouldBeNull();
        return this;
    }
}