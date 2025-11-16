using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class GeraltTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Geralt of Rivia")
        .Load("../../../../.generated/loadouts/Geralt of Rivia/Geralt of Rivia.json");

    [Theory]
    [InlineData("GEAR: Sword of Silver", "GEAR: Wolf Medallion", "GEAR: Blizzard")]
    [InlineData("GEAR: Sword of Silver", "GEAR: Wolf Medallion", "GEAR: Tawny Owl")]
    [InlineData("GEAR: Sword of Silver", "GEAR: Armor Of The Forgotten Wolf", "GEAR: Blizzard")]
    [InlineData("GEAR: Sword of Silver", "GEAR: Armor Of The Forgotten Wolf", "GEAR: Tawny Owl")]
    [InlineData("GEAR: Sword of Steel", "GEAR: Wolf Medallion", "GEAR: Blizzard")]
    [InlineData("GEAR: Sword of Steel", "GEAR: Wolf Medallion", "GEAR: Tawny Owl")]
    [InlineData("GEAR: Sword of Steel", "GEAR: Armor Of The Forgotten Wolf", "GEAR: Blizzard")]
    [InlineData("GEAR: Sword of Steel", "GEAR: Armor Of The Forgotten Wolf", "GEAR: Tawny Owl")]
    public async Task NameMe(string sword, string armor, string potion)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Geralt
            .AddNode(1, [0])                 // Dendelion
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
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
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Choose(sword)
                    .Choose(armor)
                    .Choose(potion)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
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
                // .ConfigDeck(d => d
                //     .AddBasicVersatile(3)
                // )
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
}
