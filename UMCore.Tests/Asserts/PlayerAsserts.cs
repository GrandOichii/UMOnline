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
        _controller.AttackChoices.ShouldBeEmpty();
        _controller.FighterChoices.ShouldBeEmpty();
        _controller.HandCardChoices.ShouldBeEmpty();
        _controller.NodeChoices.ShouldBeEmpty();
        _controller.StringChoices.ShouldBeEmpty();
        _controller.PathChoices.ShouldBeEmpty();
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

    public PlayerAsserts HasCardsInDiscardPile(int amount)
    {
        _player.DiscardPile.Count.ShouldBe(amount);
        return this;
    }

    public PlayerAsserts HasCardsInDeck(int amount)
    {
        _player.Deck.Count.ShouldBe(amount);
        return this;
    }
}