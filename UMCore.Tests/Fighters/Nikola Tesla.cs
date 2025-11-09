using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class NikolaTeslaTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Nikola Tesla")
        .Load("../../../../.generated/loadouts/Nikola Tesla/Nikola Tesla.json")
        .ClearDeck();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    public async Task CheckEOTCoilCharge(int turnCount, int expectedValue)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Nikola Tesla
            .AddNode(1, [0])
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(turnCount - 1, na => na.Manoeuvre())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c.NTimes(turnCount - 1, nc => nc.First()))
                .ConfigPathChoices(c => c.NTimes(turnCount - 1, nc => nc.First()))
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(turnCount - 1, na => na.Manoeuvre())
                )
                .ConfigFighterChoices(c => c.NTimes(turnCount - 1, nc => nc.First()))
                .ConfigPathChoices(c => c.NTimes(turnCount - 1, nc => nc.First()))
                .Build(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IntAttrEq("COILS", expectedValue)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CheckTurnStart()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Nikola Tesla
            .AddNode(1, [1], spawnNumber: 1) // Foo
            .AddNode(2, [2])
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(2)
                    )
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .SetIntAttr(0, "COILS", 2)
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c.First())
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .Build(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(1);
    }

}
