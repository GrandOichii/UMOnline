using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class MapNodeAsserts(MapNode node)
{
    public MapNodeAsserts HasToken(string tokenName)
    {
        node.HasToken(tokenName).ShouldBeTrue();
        return this;
    }

    public MapNodeAsserts HasNoTokens()
    {
        node.Tokens.Count.ShouldBe(0);
        return this;
    }
}