using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class PlayerAsserts(Player player)
{
    private TestPlayerController _controller = (TestPlayerController)((SafePlayerController)player.Controller).Controller;

    public PlayerAsserts SetupCalled()
    {
        _controller.SetupCalled.ShouldBeTrue();
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
}