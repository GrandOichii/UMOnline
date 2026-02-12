
public class InitialFighterPlacementTests
{
    [Theory]
    [InlineData(1, 1, new int[] { 3 })]
    [InlineData(1, 2, new int[] { 3, 2 })]
    [InlineData(1, 3, new int[] { 3, 2, 1 })]
    [InlineData(2, 0, new int[] { 3 })]
    [InlineData(2, 1, new int[] { 3, 2 })]
    [InlineData(2, 2, new int[] { 3, 2, 1 })]
    [InlineData(4, 1, new int[] { 3, 2, 1, 4 })]
    [InlineData(4, 2, new int[] { 3, 2, 1, 4, 3 })]
    [InlineData(4, 3, new int[] { 3, 2, 1, 4, 3, 2 })]
    [InlineData(4, 4, new int[] { 3, 2, 1, 4, 3, 2, 1 })]
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
                        [3, 2, 1, 4],
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
