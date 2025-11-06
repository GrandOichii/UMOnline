using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Asserts;

public class MapNodeAsserts(MapNode node)
{
    public MapNodeAsserts HasToken(string tokenName, int amount = 1)
    {
        node.Tokens.Where(t => t.GetName() == tokenName).Count().ShouldBe(amount);
        node.HasToken(tokenName).ShouldBeTrue();
        return this;
    }

    public MapNodeAsserts HasNoTokens()
    {
        node.Tokens.Count.ShouldBe(0);
        return this;
    }

    public MapNodeAsserts IsEmpty()
    {
        node.IsEmpty().ShouldBeTrue();
        return this;
    }

    public MapNodeAsserts HasFighter(string name)
    {
        node.Fighter.ShouldNotBeNull();
        node.Fighter.Name.ShouldBe(name);
        return this;
    }
}