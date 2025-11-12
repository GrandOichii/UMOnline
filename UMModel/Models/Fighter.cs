using UMCore.Templates;

namespace UMModel.Models;

public class Fighter
{
    public required string Key { get; set; }

    public required string Name { get; set; }
    public required int Amount { get; set; }
    public required string Text { get; set; }
    public required int MaxHealth { get; set; }
    public required int StartingHealth { get; set; }
    public required bool IsHero { get; set; }
    public required int Movement { get; set; }
    public required bool IsRanged { get; set; }
    public required string Script { get; set; }    
    public required bool CanMoveOverOpposing { get; set; }
    public required int MeleeRange { get; set; }

    public required string LoadoutName { get; set; }
    public required Loadout Loadout { get; set; }

    public FighterTemplate ToTemplate()
    {
        return new()
        {
            Key = Key,
            Name = Name,
            Amount = Amount,
            Text = Text,
            MaxHealth = MaxHealth,
            StartingHealth = StartingHealth,
            IsHero = IsHero,
            Movement = Movement,
            IsRanged = IsRanged,
            Script = Script,
            CanMoveOverOpposing = CanMoveOverOpposing,
            MeleeRange = MeleeRange,
        };
    }

    
    public static Fighter FromTemplate(Loadout loadout, FighterTemplate template)
    {
        return new()
        {
            Key = template.Key,
            Name = template.Name,
            Amount = template.Amount,
            Text = template.Text,
            MaxHealth = template.MaxHealth,
            StartingHealth = template.StartingHealth,
            IsHero = template.IsHero,
            Movement = template.Movement,
            IsRanged = template.IsRanged,
            Script = template.Script,
            CanMoveOverOpposing = template.CanMoveOverOpposing,
            MeleeRange = template.MeleeRange,
            Loadout = loadout,
            LoadoutName = loadout.Name,
        };
    }

    
}