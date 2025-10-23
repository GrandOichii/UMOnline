using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class AliceTests
{

    private static readonly string SIZE_ATTR = "ALICE_SIZE";

    private static readonly LoadoutTemplateBuilder LOADOUT;

    static AliceTests()
    {
        LOADOUT = new LoadoutTemplateBuilder("Alice")
            .Load("../../../../.generated/loadouts/Alice/Alice.json")
            .ClearDeck();

    }
    // test when BIG (+2 to all attacks(attack + versatile))
    // test when SMALL (+1 to defense cards(defense + versatile))

    // TODO test that Jabberwock is not effected

    [Theory]
    [InlineData("BIG")]
    [InlineData("SMALL")]
    public async Task InitialSizeBig(string targetSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

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
                .ConfigStringChoices(c => c
                    .Assert(a => a
                        .EquivalentTo(["BIG", "SMALL"])
                    )
                    .Choose(targetSize)
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            LOADOUT.Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .AttrEq(SIZE_ATTR, targetSize)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("BIG", 7, "Attack")]
    [InlineData("BIG", 7, "Versatile")]
    [InlineData("SMALL", 5, "Attack")]
    [InlineData("SMALL", 5, "Versatile")]
    public async Task CheckAttack(string size, int expectedDamage, string cardType)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0])
            .AddNode(1, [0], spawnNumber: 1)
            .AddNode(2, [0], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
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
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Assert(a => a
                        .EquivalentTo(["BIG", "SMALL"])
                    )
                    .Choose(size)
                )
                .ConfigNodeChoices(c => c
                    .WithId(0)
                )
                .Build(),
            LOADOUT
                .ClearDeck()
                .ConfigDeck(d => d
                    .AddBasicValueCard(cardType, 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
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
            .AttrEq(SIZE_ATTR, size)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(expectedDamage);
    }

    [Theory]
    [InlineData("BIG", 2, "Defense")]
    [InlineData("BIG", 2, "Versatile")]
    [InlineData("SMALL", 1, "Defense")]
    [InlineData("SMALL", 1, "Versatile")]
    public async Task CheckDefense(string size, int expectedDamage, string cardType)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayer(1)
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0])
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0], spawnNumber: 1)
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Assert(a => a
                        .EquivalentTo(["BIG", "SMALL"])
                    )
                    .Choose(size)
                )
                .ConfigNodeChoices(c => c
                    .WithId(0)
                )
                .Build(),
            LOADOUT
                .ClearDeck()
                .ConfigDeck(d => d
                    .AddBasicValueCard(cardType, 3)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigActions(c => c
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .AddBasicAttack(5)
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
            .AttrEq(SIZE_ATTR, size)
            .IsNotWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertFighter("Alice")
            .HasDamage(expectedDamage);
    }
}
