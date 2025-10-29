using NLua;

namespace UMCore.Matches.Tokens;

public class TokenManager(Match match)
{
    public Dictionary<string, Token> Tokens { get; } = [];

    public void Declare(string tokenName, Fighter fighter, LuaTable data)
    {
        if (IsDefined(tokenName))
        {
            throw new MatchException($"Tried to declare duplicate token with name {tokenName}");
        }

        var token = new Token(tokenName, fighter, data);
        Tokens.Add(tokenName, token);
    }

    public bool IsDefined(string tokenName)
    {
        return Tokens.ContainsKey(tokenName);
    }

    public Token Get(string tokenName)
    {
        return Tokens[tokenName];
    }
}