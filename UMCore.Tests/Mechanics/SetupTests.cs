
public class SetupTests
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
            .CrashedIntentionally();

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
