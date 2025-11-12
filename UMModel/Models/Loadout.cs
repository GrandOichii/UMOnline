using UMCore.Matches;
using UMCore.Templates;

namespace UMModel.Models;

public class Loadout
{
    public required string Name { get; set; }
    public required bool StartsWithSidekicks { get; set; }
    public required List<Fighter> Fighters { get; init; }
    public required List<Card> Deck { get; init; }
    public required bool ChoosesSidekick { get; init; }
    public required List<string> StartsWithCards { get; init; }
    public required List<string> CantBePlayedWith { get; init; }
    public required int? StartingHandSize { get; set; }
    public required int? MaximumHandSize { get; set; }

    public LoadoutTemplate ToTemplate()
    {
        return new()
        {
            Name = Name,
            StartsWithSidekicks = StartsWithSidekicks,
            Fighters = [.. Fighters.Select(f => f.ToTemplate())],
            Deck = [.. Deck.Select(c => c.ToTemplate())],
            ChoosesSidekick = ChoosesSidekick,
            StartsWithCards = StartsWithCards,
            CantBePlayedWith = CantBePlayedWith,
            StartingHandSize = StartingHandSize,
            MaximumHandSize = MaximumHandSize,
        };
    }

    public static Loadout FromTemplate(LoadoutTemplate template)
    {
        var result = new Loadout()
        {
            Name = template.Name,
            StartsWithSidekicks = template.StartsWithSidekicks,
            Fighters = [],
            Deck = [],
            ChoosesSidekick = template.ChoosesSidekick,
            StartsWithCards = template.StartsWithCards,
            CantBePlayedWith = template.CantBePlayedWith,
            StartingHandSize = template.StartingHandSize,
            MaximumHandSize = template.MaximumHandSize,
        };

        result.Fighters.AddRange(template.Fighters.Select(f => Fighter.FromTemplate(result, f)));
        result.Deck.AddRange(template.Deck.Select(c => Card.FromTemplate(result, c)));

        return result;
    }
}