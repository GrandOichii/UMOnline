namespace UMCore.Tests.Controllers;

public class PlaybackTests
{
    [Fact]
    public async Task ShouldPlayback()
    {
        // Arrange
        MatchConfig config = new()
        {
            ActionsPerTurn = MatchConfig.Default1x1.ActionsPerTurn,
            ExhaustDamage = MatchConfig.Default1x1.ExhaustDamage,
            FirstPlayerIdx = MatchConfig.Default1x1.FirstPlayerIdx,
            InitialHandSize = MatchConfig.Default1x1.InitialHandSize,
            ManoeuvreDrawAmount = MatchConfig.Default1x1.ManoeuvreDrawAmount,
            MaxHandSize = MatchConfig.Default1x1.MaxHandSize,
            RandomFirstPlayer = MatchConfig.Default1x1.RandomFirstPlayer,
            RandomMatch = false,
            Seed = 0,
            TeamCount = MatchConfig.Default1x1.TeamCount,
            TeamSize = MatchConfig.Default1x1.TeamSize
        };

        var map = MapTemplate.GetBaskervilleTemplate();

        var match = new Match(config, map, File.ReadAllText("../../../../core.lua"))
        {
            Logger = null
        };

        // Act


        // Asserts


    }
}