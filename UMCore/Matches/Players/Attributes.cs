namespace UMCore.Matches.Players;

public class Attributes(Player player)
{
    public Dictionary<string, string> Values { get; } = [];

    public string? Get(string key)
    {
        return Values.GetValueOrDefault(key);
    }

    public string? Set(string key, string value)
    {
        if (!Values.TryGetValue(key, out string? result)) { }
        Values[key] = value;
        return result;
    }
}