using Microsoft.Extensions.Logging;

namespace UMCore.Matches.Players;

public class TypedAttributes<T>(Player player)
{
    public Dictionary<string, T> Values { get; } = [];
    public T? Get(string key)
    {
        return Values.GetValueOrDefault(key);
    }

    public T? Set(string key, T value)
    {
        Values.TryGetValue(key, out T? result);
        Values[key] = value;
        player.Match.Logger?.LogDebug("Set player attribute {AttributeKey} to {AttributeValue}", key, value);
        return result;
    }   
}

public class Attributes(Player player)
{
    public Dictionary<string, string> Values { get; } = [];

    public TypedAttributes<string> String { get; } = new(player);    
    public TypedAttributes<int> Int { get; } = new(player);    
}