using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class PlayerAsserts
{
    private readonly Player _player;
    private readonly TestPlayerController _controller;

    public PlayerAsserts(Player player)
    {
        _player = player;
        _controller = (TestPlayerController)((SafePlayerController)player.Controller).Controller;

        // _controller.Actions.ShouldBeEmpty();
        _controller.AttackChoices.ShouldBeEmpty($"{nameof(_controller.AttackChoices)} of player {_player.LogName} was not empty");
        _controller.FighterChoices.ShouldBeEmpty($"{nameof(_controller.FighterChoices)} of player {_player.LogName} was not empty");
        _controller.HandCardChoices.ShouldBeEmpty($"{nameof(_controller.HandCardChoices)} of player {_player.LogName} was not empty");
        _controller.NodeChoices.ShouldBeEmpty($"{nameof(_controller.NodeChoices)} of player {_player.LogName} was not empty");
        _controller.StringChoices.ShouldBeEmpty($"{nameof(_controller.StringChoices)} of player {_player.LogName} was not empty");
        _controller.PathChoices.ShouldBeEmpty($"{nameof(_controller.PathChoices)} of player {_player.LogName} was not empty");
    }

    public PlayerAsserts SetupCalled()
    {
        _controller.SetupCalled.ShouldBeTrue();
        return this;
    }

    public PlayerAsserts HasFighterWithName(string name)
    {
        _player.Fighters.FirstOrDefault(f => f.Name == name).ShouldNotBeNull();
        return this;
    }

    public PlayerAsserts CombinedFighterHealthEq(int amount)
    {
        var health = _player.Fighters.Sum(f => f.Health.Current);
        health.ShouldBe(amount);
        return this;
    }

    public PlayerAsserts DoesntHaveFighterWithName(string name)
    {
        _player.Fighters.FirstOrDefault(f => f.Name == name).ShouldBeNull();
        return this;
    }

    public PlayerAsserts StringAttrEq(string attrKey, string value)
    {
        var attr = _player.Attributes.String.Get(attrKey);
        attr.ShouldNotBeNull();
        attr.ShouldBe(value);
        return this;
    }

    public PlayerAsserts IntAttrEq(string attrKey, int value)
    {
        var attr = _player.Attributes.Int.Get(attrKey);
        attr.ShouldBe(value);
        return this;
    }

    public PlayerAsserts IsCurrentPlayer()
    {
        _player.Match.CurPlayerIdx.ShouldBe(_player.Idx);
        return this;
    }

    public PlayerAsserts IsWinner()
    {
        _player.Match.Winner.ShouldBe(_player);
        return this;
    }

    public PlayerAsserts IsNotWinner()
    {
        _player.Match.Winner.ShouldNotBe(_player);
        return this;
    }

    public PlayerAsserts HasUnspentActions(int amount)
    {
        _player.ActionCount.ShouldBe(amount);
        return this;
    }

    public PlayerAsserts HasNoUnspentActions()
    {
        _player.ActionCount.ShouldBe(0);
        return this;
    }

    public PlayerAsserts HasCardsInHand(int amount)
    {
        _player.Hand.Count.ShouldBe(amount);
        return this;
    }
    
    public PlayerAsserts HasCardInHand(string key)
    {
        _player.Hand.Cards.FirstOrDefault(c => c.Template.Key == key).ShouldNotBeNull();
        return this;
    }

    public PlayerAsserts HasCardsInDiscardPile(int amount)
    {
        return HasCardsInZone("DISCARD", amount);
    }

    public PlayerAsserts HasCardsInZone(string zoneName, int amount)
    {
        var zone = _player.CardZones[zoneName];
        zone.Count.ShouldBe(amount, $"Expected card zone {zoneName} of player {_player.LogName} to have {amount} cards, while it has {zone.Count}");
        return this;
    }

    public PlayerAsserts HasCardsInDeck(int amount)
    {
        return HasCardsInZone("DECK", amount);
    }

    public PlayerAsserts DoesntHaveCardZone(string name)
    {
        _player.CardZones.ShouldNotContainKey(name);
        return this;
    }

    public PlayerAsserts HasCardZone(string name)
    {
        _player.CardZones.ShouldContainKey(name);
        return this;
    }
}