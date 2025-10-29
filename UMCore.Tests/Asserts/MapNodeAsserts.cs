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
}