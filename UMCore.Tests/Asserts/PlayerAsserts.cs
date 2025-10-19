using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class PlayerAsserts(Player player)
{
    private readonly TestPlayerController _controller = (TestPlayerController)((SafePlayerController)player.Controller).Controller;

    public PlayerAsserts SetupCalled()
    {
        _controller.SetupCalled.ShouldBeTrue();
        return this;
    }

    public PlayerAsserts IsCurrentPlayer()
    {
        player.Match.CurPlayerIdx.ShouldBe(player.Idx);
        return this;
    }

    public PlayerAsserts IsWinner()
    {
        player.Match.Winner.ShouldBe(player);
        return this;
    }

    public PlayerAsserts IsNotWinner()
    {
        player.Match.Winner.ShouldNotBe(player);
        return this;
    }

    public PlayerAsserts HasUnspentActions(int amount)
    {
        player.ActionCount.ShouldBe(amount);
        return this;
    }

    public PlayerAsserts HasNoUnspentActions()
    {
        player.ActionCount.ShouldBe(0);
        return this;
    }

    public PlayerAsserts HasCardsInHand(int amount)
    {
        player.Hand.Count.ShouldBe(amount);
        return this;
    }

    public PlayerAsserts HasCardsInDiscardPile(int amount)
    {
        player.DiscardPile.Count.ShouldBe(amount);
        return this;
    }

    public PlayerAsserts HasCardsInDeck(int amount)
    {
        player.Deck.Count.ShouldBe(amount);
        return this;
    }
}