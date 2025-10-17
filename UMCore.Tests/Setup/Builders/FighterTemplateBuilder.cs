namespace UMCore.Tests.Setup.Builders;

public class FighterTemplateBuilder(string name, string key)
{
    public static readonly string DEFAULT_FIGHTER_SCRIPT = "function _Create() return UM.Build:Fighter():Build() end";

    public FighterTemplate Result { get; } = new()
    {
        Amount = 1,
        IsHero = true,
        IsRanged = false,
        Key = key,
        MaxHealth = 10,
        StartingHealth = 10,
        Movement = 2,
        MeleeRange = 1,
        Name = name,
        Script = DEFAULT_FIGHTER_SCRIPT,
        Text = "",
    };

    public FighterTemplate Build()
    {
        return Result;
    }

    public FighterTemplateBuilder Health(int health)
    {
        Result.MaxHealth = health;
        Result.StartingHealth = health;
        return this;
    }

    public FighterTemplateBuilder IsSidekick()
    {
        Result.IsHero = false;
        return this;
    }

    public FighterTemplateBuilder Movement(int amount)
    {
        Result.Movement = amount;
        return this;
    }
}