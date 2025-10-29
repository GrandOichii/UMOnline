using Shouldly;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;

namespace UMCore.Tests.Asserts;

public class TokenAsserts(Token token)
{
    public TokenAsserts HasAmount(int v)
    {
        token.Amount.ShouldBe(v);
        return this;
    }
}