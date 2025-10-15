using System.Threading.Tasks;
using Shouldly;
using UMCore.Tests.Setup;

namespace UMCore.Tests.Matches;

public class PlayerAddingTests
{
    [Fact]
    public async Task CantRunWithoutPlayers()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedUnintentionally()
            .CantRun();
    }

    [Fact]
    public async Task CantRunWithOnePlayer()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedUnintentionally()
            .CantRun();
    }

    [Fact]
    public async Task CantAddWithSameLoadout()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo()
        );

        // Assert
        result.ShouldBeFalse();
        match.Assert()
            .CantRun()
            .PlayerCount(1);
    }

    [Fact]
    public async Task CantAddMoreThanTeamSize()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(1)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo1")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo3")
        );

        // Assert
        result.ShouldBeFalse();
        match.Assert()
            .CanRun()
            .PlayerCount(2);
    }

    [Fact]
    public async Task CantStartGameWithUnbalancedTeams()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo3")
        );

        // Assert
        match.Assert()
            .CantRun()
            .PlayerCount(3);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CanRunWithTeamSizes(int teamSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(teamSize)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        // Act
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddMainPlayer(
                TestPlayerControllerBuilder.Crash(),
                LoadoutTemplateBuilder.Foo($"foo{i}")
            );
        }
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddOpponent(
                TestPlayerControllerBuilder.Crash(),
                LoadoutTemplateBuilder.Foo($"bar{i}")
            );
        }

        // Assert
        match.Assert()
            .CanRun()
            .PlayerCount(teamSize * 2);
    }
}

public class TODOSortTheseTests
{
    [Fact]
    public async Task SetupCalledForPlayers()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally(); // TODO could change

        match.AssertPlayer(0)
            .SetupCalled();
        match.AssertPlayer(1)
            .SetupCalled();
    }

    [Fact]
    public async Task ShouldRunBase()
    {
        // Arrange
        var config = new MatchConfigBuilder()
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
                .Build(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}

public class MovementTests
{    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task LineMovementTests(int fighterMovement)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
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
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .AssertOptionsHasLength(fighterMovement + 1)
                    .WithId(0))
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task LineMovementWithBoostTests(int boostValue)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(5)
            .Build();

        // 0 - 1 - 2 - 3 - 4
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
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
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .AssertOptionsHasLength(boostValue + 1)
                    .WithId(0))
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(0)
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(boostValue, 10)
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .HasCardsInHand(5)
            .HasCardsInDeck(4)
            .HasCardsInDiscardPile(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task LineWithSecretPassageMovementTests(int fighterMovement)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], true, spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], true)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
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
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .AssertOptionsHasLength(fighterMovement + 1)
                    .WithId(0))
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}