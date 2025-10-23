using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class AliceTests {
    private static LoadoutTemplate LoadLoadout(string path)
    {
        var data = File.ReadAllText(path);
        var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
        foreach (var card in result.Deck)
        {
            card.Card.Script = File.ReadAllText($"../../../../{card.Card.Script}");
        }

        foreach (var fighter in result.Fighters)
        {
            fighter.Script = File.ReadAllText($"../../../../{fighter.Script}");
        }
        return result;
    }
    
    private static LoadoutTemplate LOADOUT;

    static AliceTests()
    {
        LOADOUT = LoadLoadout("../../../../.generated/loadouts/Alice/Alice.json");
        // LOADOUT

    }
        // small
    // test when BIG (+2 to all attacks(attack + versatile))
    // test when SMALL (+1 to defense cards(defense + versatile))
    
// -- When you place Alice, choose whether she starts the game BIG or SMALL.
// -- When Alice is BIG, add 2 to the value of her attack cards.
// -- When Alice is SMALL, add 1 to the value of her defense cards.

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
            LOADOUT
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
            .AttrEq("ALICE_SIZE", targetSize)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}