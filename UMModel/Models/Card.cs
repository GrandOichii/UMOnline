using UMCore.Templates;

namespace UMModel.Models;

public class Card
{
    public required string Key { get; set; }
    public required int Amount { get; set; }   
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required int? Value { get; set; }
    public required int? Boost { get; set; }
    public required string Text { get; set; }
    public required string Script { get; set; }
    public required string[] AllowedFighters { get; set; }
    public required string[] Labels { get; set; }
    public required string? IncludedInDeckWithSidekick { get; set; }

    public required string LoadoutName { get; set; }
    public required Loadout Loadout { get; set; }

    public CardTemplate ToTemplate()
    {
        return new()
        {
            Key = Key,
            Amount = Amount,
            Name = Name,
            Type = Type,
            Value = Value,
            Boost = Boost,
            Text = Text,
            Script = Script,
            AllowedFighters = AllowedFighters,
            Labels = Labels,
            IncludedInDeckWithSidekick = IncludedInDeckWithSidekick,
        };
    }

    public static Card FromTemplate(Loadout loadout, CardTemplate template)
    {
        return new()
        {
            Key = template.Key,
            Amount = template.Amount,
            Name = template.Name,
            Type = template.Type,
            Value = template.Value,
            Boost = template.Boost,
            Text = template.Text,
            Script = template.Script,
            AllowedFighters = template.AllowedFighters,
            Labels = template.Labels,
            IncludedInDeckWithSidekick = template.IncludedInDeckWithSidekick,
            Loadout = loadout,
            LoadoutName = loadout.Name,
        };
    }
}