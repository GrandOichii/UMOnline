
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
            .ConfigCardChoices(c => c
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
            .ConfigCardChoices(c => c
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

