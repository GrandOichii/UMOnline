using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class MoonKnightTests
{
    private static readonly string IDENTITY_ATTR = "IDENTITY";

    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Moon Knight")
        .Load("../../../../.generated/loadouts/Moon Knight/Moon Knight.json")
        .ClearDeck();

    [Theory]
    [InlineData(1, "Moon Knight")]
    [InlineData(2, "Khonshu")]
    [InlineData(3, "Mr. Knight")]
    [InlineData(4, "Moon Knight")]
    [InlineData(5, "Khonshu")]
    [InlineData(6, "Mr. Knight")]
    public async Task IdentityCycling(int amountOfTurns, string expectedIdentity)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
                .ConfigPathChoices(c => c
                    .NTimes((int)Math.Ceiling(amountOfTurns / 3.0), nc => nc.First()) // this tests Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
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
            .StringAttrEq(IDENTITY_ATTR, expectedIdentity)
            .HasFighterWithName(expectedIdentity)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    public async Task NotKhonshuAttack(int amountOfTurns)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
                .ConfigPathChoices(c => c
                    .NTimes((int)Math.Ceiling(amountOfTurns / 3.0), nc => nc.First()) // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
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
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertFighter("Foo")
            .HasDamage(5);
    }

    [Fact]
    public async Task KhonshuAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First() // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
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
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertFighter("Foo")
            .HasDamage(7);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    public async Task NotKhonshuNonCombatDamage(int amountOfTurns)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .DealDamage("Moon Knight", 1, true)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
                .ConfigPathChoices(c => c
                    .NTimes((int)Math.Ceiling(amountOfTurns / 3.0), nc => nc.First()) // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Moon Knight")
            .HasDamage(1);
    }

    [Fact]
    public async Task KhonshuNonCombatDamage()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .DealDamage("Moon Knight", 1, true)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First() // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Scheme()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Moon Knight")
            .IsAtFullHealth();
    }

    [Fact]
    public async Task KhonshuCombatDamage()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                    .Nothing()
                )
                .ConfigPathChoices(c => c
                    .First() // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicAttack(1, amount: 10)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Moon Knight")
            .HasDamage(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task NonMrKnight_Defense(int amountOfTurns)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                    .First()
                )
                .ConfigPathChoices(c => c
                    .NTimes((int)Math.Ceiling((amountOfTurns) / 3.0), nc => nc.First()) // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicDefense(3, amount: 5)
                )
                .Build()
        );

        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .Attack()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Moon Knight")
            .DoesntHaveName("Mr. Knight")
            .HasDamage(5 - 3);
    }

    [Fact]
    public async Task MrKnight_Defense()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(2, nc => nc.Scheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(2, nc => nc.First())
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First() // this tests Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicDefense(3, amount: 5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .NTimes(1, nc => nc.Scheme())
                    .Attack()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(1, nc => nc.First())
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
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
            .StringAttrEq(IDENTITY_ATTR, "Mr. Knight")
            .HasFighterWithName("Mr. Knight")
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Moon Knight")
            .HasDamage(5 - 3 - 1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task NoDefenseCard(int amountOfTurns)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(10)
            .MaxHandSize(10)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                    .Nothing()
                )
                .ConfigPathChoices(c => c
                    .NTimes((int)Math.Ceiling((amountOfTurns) / 3.0), nc => nc.First()) // Moon Knight's turn start movement
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
                )
                .Build()
        );

        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.Scheme())
                    .Attack()
                )
                .ConfigHandCardChoices(c => c
                    .NTimes(amountOfTurns - 1, nc => nc.First())
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 5)
                    .AddBasicAttack(5, amount: 5)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Moon Knight")
            .HasDamage(5);
    }
}
