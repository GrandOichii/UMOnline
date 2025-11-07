using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class DrSattlerTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Dr. Ellie Sattler")
        .Load("../../../../.generated/loadouts/Dr. Ellie Sattler/Dr. Ellie Sattler.json")
        .ClearDeck();

    [Fact]
    public async Task CheckTriggerPlacement()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Dr. Sattler
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])                 // Dr. Malcolm
            .AddNode(4, [0])                 
            .AddNode(5, [0])                 
            .AddNode(6, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(3, na => na.Manoeuvre())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(1)
                    .FirstStopsAtId(4)
                    .FirstStopsAtId(2)
                    .FirstStopsAtId(5)
                    .FirstStopsAtId(0)
                    .FirstStopsAtId(3)
                )
                .ConfigFighterChoices(c => c
                    // .NTimes(2, nc => nc.First())
                    .NTimes(3, nc => nc
                        .WithName("Dr. Sattler")
                        .WithName("Dr. Malcolm")
                    )
                )
                .ConfigNodeChoices(c => c
                    .WithId(3)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(2, na => na.Manoeuvre())
                )
                .ConfigFighterChoices(c => c
                    .NTimes(2, nc => nc.First())
                )
                .ConfigPathChoices(c => c
                    .NTimes(2, nc => nc.First())
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

        // for (int i = 0; i <= 4; ++i)
        //     match.AssertNode(i)
        //         .HasToken("Insight");
        match.AssertNode(0)
            .HasToken("Insight");
    }
}
