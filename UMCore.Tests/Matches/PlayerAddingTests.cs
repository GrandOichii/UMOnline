using System.ComponentModel;
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
            TestPlayerControllerBuilder.Crasher(),
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
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo1")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
                TestPlayerControllerBuilder.Crasher(),
                LoadoutTemplateBuilder.Foo($"foo{i}")
            );
        }
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddOpponent(
                TestPlayerControllerBuilder.Crasher(),
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
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
            TestPlayerControllerBuilder.Crasher(),
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
    // TODO add tests where player cant discard cards for boost (they dont have boost values)

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 7)]
    [InlineData(4, 12)]
    [InlineData(5, 20)]
    public async Task LineMovementTests(int fighterMovement, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2)
            .AddNode(5, [0])
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
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 7)]
    [InlineData(4, 12)]
    [InlineData(5, 20)]
    public async Task LineMovementWithBoostTests(int boostValue, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(5)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
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
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                )
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
            TestPlayerControllerBuilder.Crasher(),
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
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(3, 4)]
    [InlineData(4, 9)]
    [InlineData(5, 12)]
    public async Task LineMovementWithSidekickTests(int boostValue, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(5)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // hero is here
            .AddNode(1, [0])                 // sidekick is here
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2) // opponent is here
            .AddNode(5, [0])
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
                    .WithName("foo1")
                    .WithName("bar1")
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(0)
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("bar1", "bar1")
                    .IsSidekick()
                    .Movement(0)
                    .Build())
                .ConfigDeck(d => d
                    .AddBasicScheme(boostValue, 10)
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 7)]
    [InlineData(4, 12)]
    public async Task LineWithSecretPassageMovementTests(int fighterMovement, int expectedPathsCount)
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
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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
    public async Task LineWithSurroundedTests(int fighterMovement)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // opponent hero
            .AddNode(1, [0], spawnNumber: 1) // main hero
            .AddNode(2, [0])                 // opponent sidekick
            .AddNode(3, [0])
            .AddNode(4, [0])
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
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a.OptionsCount(1))
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(2))
                .Build(),
            LoadoutTemplateBuilder.FooBar("foo2")
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

public class InitialFighterPlacementTests
{
    [Theory]
    [InlineData(1, 1, new int[] { 3 })]
    [InlineData(1, 2, new int[] { 3, 2 })]
    [InlineData(1, 3, new int[] { 3, 2, 1 })]
    [InlineData(2, 0, new int[] { 3 })]
    [InlineData(2, 1, new int[] { 3, 2 })]
    [InlineData(2, 2, new int[] { 3, 2, 1 })]
    [InlineData(4, 1, new int[] { 3, 2, 1, 5 })]
    [InlineData(4, 2, new int[] { 3, 2, 1, 5, 4 })]
    [InlineData(4, 3, new int[] { 3, 2, 1, 5, 4, 3 })]
    [InlineData(4, 4, new int[] { 3, 2, 1, 5, 4, 3, 2 })]
    public async Task Place_N_Heroes_M_Sidekicks_In_3x3_Box_2x2_Zone(int heroCount, int sidekickCount, int[] optionCounts)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 00 - 01 - 02
        // |  X |  X |
        // 10 - 11 - 12
        // |  X |  X |
        // 20 - 21 - 22
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(00, [0], spawnNumber: 1)
            .AddNode(01, [0])
            .AddNode(10, [0])
            .AddNode(11, [0])
            .AddNode(02, [1])
            .AddNode(12, [1])
            .AddNode(22, [1], spawnNumber: 2)
            .AddNode(21, [1])
            .AddNode(20, [1])
            .ConnectAll()
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
                .ConfigNodeChoices(c => c
                    .ForEach(
                        optionCounts,
                        (cnc, o) => cnc
                            .AssertOptionsHasLength(o)
                            .First()
                    )
                )
                .Build(),
            LoadoutTemplateBuilder.NHeroesMSidekicks(heroCount, sidekickCount)
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
            .HasUnspentActions(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(1, 1, new int[] { 3 })]
    [InlineData(1, 2, new int[] { 3, 2 })]
    [InlineData(1, 3, new int[] { 3, 2, 1 })]
    [InlineData(2, 0, new int[] { 3 })]
    [InlineData(2, 1, new int[] { 3, 2 })]
    [InlineData(2, 2, new int[] { 3, 2, 1 })]
    public async Task Opponent_Place_N_Heroes_M_Sidekicks_In_3x3_Box_2x2_Zone(int heroCount, int sidekickCount, int[] optionCounts)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 00 - 01 - 02
        // |  X |  X |
        // 10 - 11 - 12
        // |  X |  X |
        // 20 - 21 - 22
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(00, [0], spawnNumber: 1)
            .AddNode(01, [0])
            .AddNode(10, [0])
            .AddNode(11, [0])
            .AddNode(02, [1])
            .AddNode(12, [1])
            .AddNode(22, [1], spawnNumber: 2)
            .AddNode(21, [1])
            .AddNode(20, [1])
            .ConnectAll()
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
                .ConfigNodeChoices(c => c
                    .ForEach(
                        [3, 2, 1, 5],
                        (cnc, o) => cnc
                            .AssertOptionsHasLength(o)
                            .First()
                    )
                )
                .Build(),
            LoadoutTemplateBuilder.NHeroesMSidekicks(2, 3)
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .ForEach(
                        optionCounts,
                        (cnc, o) => cnc
                            .AssertOptionsHasLength(o)
                            .First()
                    )
                )
                .Build(),
            LoadoutTemplateBuilder.NHeroesMSidekicks(heroCount, sidekickCount)
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

public class ManoeuvreTests
{
    [Theory]
    [InlineData(10, 2)]
    [InlineData(30, 4)]
    [InlineData(1, 1)]
    public async Task SingleManoeuvre_CardDrawn_EveryFighterMoved(int deckSize, int sidekickCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        // 00 - 01 - 02
        // |  X |  X |
        // 10 - 11 - 12
        // |  X |  X |
        // 20 - 21 - 22
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(00, [0], spawnNumber: 1)
            .AddNode(01, [0])
            .AddNode(10, [0])
            .AddNode(11, [0])
            .AddNode(02, [1])
            .AddNode(12, [1])
            .AddNode(22, [1], spawnNumber: 2)
            .AddNode(21, [1])
            .AddNode(20, [1])
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var c = new TestPlayerControllerBuilder()
            .ConfigActions(a => a
                .Manoeuvre()
                .DeclareWinner()
                .CrashMatch()
            )
            .ConfigFighterChoices(c => c
                .NTimes(sidekickCount + 1, nc => nc.First())
            )
            .ConfigNodeChoices(c => c
                .NTimes(sidekickCount, nc => nc.First())
            )
            .ConfigPathChoices(c => c
                .NTimes(sidekickCount + 1, nc => nc.First())
            )
            .ConfigHandCardChoices(c => c
                .Nothing()
            )
            .Build();
        await match.AddMainPlayer(
            c,
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("hero", "hero")
                    .Build()
                )
                .ForReach(Enumerable.Range(0, sidekickCount), (ltb, _) => ltb
                    .AddFighter(new FighterTemplateBuilder("sidekick", "sidekick")
                        .IsSidekick()
                        .Build()
                    )
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: deckSize)
                )
                .Build()
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
        c.AssertAllChoiceQueuesEmpty();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .HasCardsInHand(1)
            .HasCardsInDeck(deckSize - 1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(10, 2)]
    [InlineData(30, 4)]
    [InlineData(2, 1)]
    public async Task DoubleManoeuvre_CardDrawn_EveryFighterMoved(int deckSize, int sidekickCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        // 00 - 01 - 02
        // |  X |  X |
        // 10 - 11 - 12
        // |  X |  X |
        // 20 - 21 - 22
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(00, [0], spawnNumber: 1)
            .AddNode(01, [0])
            .AddNode(10, [0])
            .AddNode(11, [0])
            .AddNode(02, [1])
            .AddNode(12, [1])
            .AddNode(22, [1], spawnNumber: 2)
            .AddNode(21, [1])
            .AddNode(20, [1])
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var c = new TestPlayerControllerBuilder()
            .ConfigActions(a => a
                .Manoeuvre()
                .Manoeuvre()
                .CrashMatch()
            )
            .ConfigFighterChoices(c => c
                .NTimes(sidekickCount + 1, nc => nc.First())
                .NTimes(sidekickCount + 1, nc => nc.First())
            )
            .ConfigNodeChoices(c => c
                .NTimes(sidekickCount, nc => nc.First())
            )
            .ConfigPathChoices(c => c
                .NTimes(sidekickCount + 1, nc => nc.First())
                .NTimes(sidekickCount + 1, nc => nc.First())
            )
            .ConfigHandCardChoices(c => c
                .Nothing()
                .Nothing()
            )
            .Build();
        await match.AddMainPlayer(
            c,
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("hero", "hero")
                    .Build()
                )
                .ForReach(Enumerable.Range(0, sidekickCount), (ltb, _) => ltb
                    .AddFighter(new FighterTemplateBuilder("sidekick", "sidekick")
                        .IsSidekick()
                        .Build()
                    )
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: deckSize)
                )
                .Build()
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
        c.AssertAllChoiceQueuesEmpty();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(0)
            .HasCardsInHand(2)
            .HasCardsInDeck(deckSize - 2);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsCurrentPlayer()
            .IsNotWinner();
    }
}

public class SchemeTests
{
    [Fact]
    public async Task CantPlaySchemeIfNoSchemeInHand()
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
                    .Assert(a => a.CantScheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            LoadoutTemplateBuilder.Foo("main")
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
            .IsWinner()
            .HasUnspentActions(2);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CanPlaySchemeOnce()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Scheme()
                    .Assert(a => a.CantScheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
                .Build()
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
            .IsWinner()
            .HasUnspentActions(1);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CanPlaySchemeTwice()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(3)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
                .Build()
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
            .IsWinner()
            .HasUnspentActions(1);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CardDrawSchemeCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d.AddCardDrawScheme(1, amount: 10))
                .Build()
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
            .IsWinner()
            .HasCardsInHand(1)
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(8)
            .HasUnspentActions(1);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}

public class ExhaustionTests
{
    [Fact]
    public async Task ManoeuvreExhaust()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .ExhaustDamage(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var heroKey = "hero";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("hero", heroKey)
                    .Health(10)
                    .Build()
                )
                .Build()
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
            .HasUnspentActions(1)
            .HasCardsInHand(0)
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(0)
            .IsWinner();
        match.AssertFighter(heroKey)
            .HasHealth(8)
            .IsAlive();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CardDrawSchemeExhaust()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .ExhaustDamage(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var heroKey = "main";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", heroKey).Build())
                .ConfigDeck(d => d.AddCardDrawScheme(1, amount: 1))
                .Build()
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
            .HasUnspentActions(1)
            .HasCardsInHand(0)
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(1)
            .IsWinner();
        match.AssertFighter(heroKey)
            .HasHealth(8)
            .IsAlive();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}

public class AttackTests
{
    [Fact]
    public async Task CantMeleeAttackSingleOpponent()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a
                        .CantAttack()
                    )
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
                .ConfigDeck(d => d.AddBasicAttack(5, amount: 10))
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
                .Build()
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

        match.AssertFighter(mainFighter)
            .IsAtFullHealth()
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAtFullHealth()
            .IsAlive();
    }

    [Fact]
    public async Task CantMeleeAttackTeammate()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .TeamSize(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0], spawnNumber: 3)
            .AddNode(3, [0], spawnNumber: 4)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main1";
        var opponentFighter = "opp1";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a
                        .CantAttack()
                    )
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            new LoadoutTemplateBuilder("main1")
                .AddFighter(new FighterTemplateBuilder("main1", mainFighter).Build())
                .ConfigDeck(d => d.AddBasicAttack(5, amount: 10))
                .Build()
        );
        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("main2")
                .AddFighter(new FighterTemplateBuilder("main2", "main2").Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("opp1")
                .AddFighter(new FighterTemplateBuilder("opp1", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("opp2")
                .AddFighter(new FighterTemplateBuilder("opp2", "opp2").Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
                .Build()
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

        match.AssertFighter(mainFighter)
            .IsAtFullHealth()
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAtFullHealth()
            .IsAlive();
    }

    [Fact]
    public async Task CantMeleeAttackWithoutAttackCards()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a
                        .CantAttack()
                    )
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
                .Build()
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

        match.AssertFighter(mainFighter)
            .IsAtFullHealth()
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAtFullHealth()
            .IsAlive();
    }

    [Fact]
    public async Task CantRangedAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [1], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a
                        .CantAttack()
                    )
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", mainFighter).IsRanged().Build())
                .ConfigDeck(d => d.AddBasicAttack(5, amount: 10))
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
                .Build()
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

        match.AssertFighter(mainFighter)
            .IsAtFullHealth()
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAtFullHealth()
            .IsAlive();
    }

    [Fact]
    public async Task CanMeleeAttack_1Main_1Opp_1Card()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .Assert(a => a
                        .OptionsCount(1)
                        .CanAttackOnly(mainFighter, opponentFighter, attackCard)
                    )
                    .First()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAlive();
    }

    [Fact]
    public async Task CanMeleeAttack_1Main_1Opp_2Cards()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .Assert(a => a
                        .OptionsCount(2)
                        .CanAttackOnly(mainFighter, opponentFighter, attackCard)
                    )
                    .First()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAlive();
    }

    [Fact]
    public async Task CanMeleeAttack_1Main_2Opp_1Card()
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

        var mainFighter = "main";
        var opponentFighter = "opp";
        var opponentSidekick = "opp-sidekick";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .Assert(a => a
                        .OptionsCount(2)
                        .CanAttack(mainFighter, opponentFighter, attackCard)
                        .CanAttack(mainFighter, opponentSidekick, attackCard)
                    )
                    .First()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigNodeChoices(c => c
                    .WithId(0)
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .AddFighter(new FighterTemplateBuilder("opp-sidekick", opponentSidekick).IsSidekick().Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAlive();
    }

    [Fact]
    public async Task CanMeleeAttack_2Main_1Opp_1Card()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var mainSidekick = "main-sidekick";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
            .AddFighter(new FighterTemplateBuilder("main-sidekick", mainSidekick).IsSidekick().Build())
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .Assert(a => a
                        .OptionsCount(2)
                        .CanAttack(mainFighter, opponentFighter, attackCard)
                        .CanAttack(mainSidekick, opponentFighter, attackCard)
                    )
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(2)
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAlive();
    }

    [Fact]
    public async Task CanRangedAttack_1Main_1Opp_1Card()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [1])
            .AddNode(2, [0], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).IsRanged().Build())
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .Assert(a => a
                        .OptionsCount(1)
                        .CanAttackOnly(mainFighter, opponentFighter, attackCard)
                    )
                    .First()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAlive();
    }

    [Fact]
    public async Task CanRangedAttackInDifferentZone_1Main_1Opp_1Card()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [1], spawnNumber: 2)
            .AddNode(2, [1])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).IsRanged().Build())
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .Assert(a => a
                        .OptionsCount(1)
                        .CanAttackOnly(mainFighter, opponentFighter, attackCard)
                    )
                    .First()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .IsAlive();
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task CheckCombatDamage_WithoutDefense(int attackValue)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
            .ConfigDeck(d => d.AddBasicAttack(attackValue, amount: 10, key: attackCard))
            .Build();

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
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(1, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAtFullHealth()
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .HasDamage(attackValue)
            .IsAlive();
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(2, 1, 1)]
    [InlineData(3, 2, 1)]
    [InlineData(3, 1, 2)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 0)]
    [InlineData(1, 2, 0)]
    public async Task CheckCombatDamage_WithDefense(int attackValue, int defenseValue, int expectedDamage)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter).Build())
            .ConfigDeck(d => d.AddBasicAttack(attackValue, amount: 10, key: attackCard))
            .Build();

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
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(defenseValue, amount: 10))
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

        match.AssertFighter(mainFighter)
            .IsAtFullHealth()
            .IsAlive();
        match.AssertFighter(opponentFighter)
            .HasDamage(expectedDamage)
            .IsAlive();
    }

    [Fact]
    public async Task CombatRange0()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter)
                .MeleeRange(0)
                .Build()
            )
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a.CantAttack())
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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
    }

    [Fact]
    public async Task CombatRange2()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [1])
            .AddNode(2, [2], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter)
                .MeleeRange(2)
                .Build()
            )
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a.CanAttack())
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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
    }

    [Fact]
    public async Task CombatRange5()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [1])
            .AddNode(2, [2])
            .AddNode(3, [3])
            .AddNode(4, [4])
            .AddNode(5, [5], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var mainFighter = "main";
        var opponentFighter = "opp";
        var attackCard = "attack";

        var loadout = new LoadoutTemplateBuilder("main")
            .AddFighter(new FighterTemplateBuilder("main", mainFighter)
                .MeleeRange(5)
                .Build()
            )
            .ConfigDeck(d => d.AddBasicAttack(5, amount: 10, key: attackCard))
            .Build();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Assert(a => a.CanAttack())
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            new LoadoutTemplateBuilder("opp")
                .AddFighter(new FighterTemplateBuilder("opp", opponentFighter).Build())
                .ConfigDeck(d => d.AddBasicDefense(3, amount: 10))
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
    }

    // TODO add tests for dead figthers
}