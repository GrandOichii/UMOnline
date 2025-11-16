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

    private bool HasNamedFighter(string name)
    {
        if (node.Fighter is not null && node.Fighter.Name == name) return true;
        var small = node.SmallFighters.FirstOrDefault(f => f.Name == name);
        return small is not null;
    }

    public MapNodeAsserts HasFighterWithName(string name)
    {
        HasNamedFighter(name).ShouldBeTrue($"Expected node with Id = {node.Id} to contain fighter with name {name}");
        return this;
    }

    public MapNodeAsserts FightersCount(int amount)
    {
        node.GetFighters().Count().ShouldBe(amount);
        return this;
    }

    public MapNodeAsserts DoesntHaveFighterWithName(string name)
    {
        HasNamedFighter(name).ShouldBeFalse($"Expected node with Id = {node.Id} not to contain fighter with name {name}");
        return this;
    }
}