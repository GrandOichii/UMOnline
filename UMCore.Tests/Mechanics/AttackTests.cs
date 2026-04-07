namespace UMCore.Tests.Mechanics;

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
            .IsWinner();

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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
                .ConfigCardChoices(c => c
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
