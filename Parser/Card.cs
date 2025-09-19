using System.Text.Json.Serialization;

namespace ScriptParser;

public class Card {
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}