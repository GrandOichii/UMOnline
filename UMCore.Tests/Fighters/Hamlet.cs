using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class HamletTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Hamlet")
        .Load("../../../../.generated/loadouts/Hamlet/Hamlet.json")
        .ClearDeck();

    [Theory]
    [InlineData("Hamlet", 2)]
    [InlineData("Rosencrantz & Guildenstern", 0)]
    public async Task NotToBe_Attack(string notToBeDamageTargetAndAttacker, int expectedFooDamage)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Hamlet
            .AddNode(1, [0])                 // R & G
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(notToBeDamageTargetAndAttacker)
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Choose("NOT TO BE")
                )
                .ConfigFighterChoices(c => c
                    .WithName(notToBeDamageTargetAndAttacker)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(0)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c.Nothing())
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .Build()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .StringAttrEq("TOBE", "NOT TO BE")
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter(notToBeDamageTargetAndAttacker)
            .HasDamage(2);
        match.AssertFighter("Foo")
            .HasDamage(expectedFooDamage);
    }

    [Fact]
    public async Task ToBe_Manoeuvre()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Hamlet
            .AddNode(1, [0])                 // R & G
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Choose("TO BE")
                )
                .ConfigHandCardChoices(c => c.Nothing())
                .ConfigFighterChoices(c => c.NTimes(2, nc => nc.First()))
                .ConfigPathChoices(c => c.NTimes(2, nc => nc.First()))
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(0, amount: 2)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .Build()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .StringAttrEq("TOBE", "TO BE")
            .HasCardsInHand(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Hamlet")
            .IsAtFullHealth();
        match.AssertFighter("Rosencrantz & Guildenstern")
            .IsAtFullHealth();
    }
}
