using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class CiriTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Ciri")
        .Load("../../../../.generated/loadouts/Ciri/Ciri.json")
        .ClearDeck();

    [Theory]
    [InlineData(0, true, 0)]
    [InlineData(0, false, 0)]
    [InlineData(6, true, 0)]
    [InlineData(6, false, 0)]
    [InlineData(7, true, 1)]
    [InlineData(7, false, 0)]
    [InlineData(10, true, 1)]
    [InlineData(10, false, 0)]
    public async Task CancelCheck_Ciri(int sourceCardsInDiscard, bool areSource, int expectedHandCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Little Red Riding Hood
            .AddNode(1, [0])                 // Ihuarraquax
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
                    .Mill(sourceCardsInDiscard)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Ciri")
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
                        .Name("sourcecard")
                        .Value(5)
                        .Versatile()
                        .CanBePlayedBy("Ciri")
                        .HasLabel(areSource ? "source" : "nonsource")
                        .Amount(sourceCardsInDiscard + 2)
                        .Script("""
                        :AfterCombat(
                        'After combat: Draw 1 card',
                        UM.Effects:Draw(
                        UM.Select:Players()
                        :You()
                        :Build(), 
                        UM.Number:Static(1), 
                        false
                        )
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
                        .Feint()
                        .Versatile()
                        .Value(3)
                        .Amount(1)                    
                        .Build()
                    )
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
            .HasCardsInHand(expectedHandCount)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
