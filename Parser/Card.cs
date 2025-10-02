using System.Text.Json.Serialization;

namespace Parser;

public class Card {
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}