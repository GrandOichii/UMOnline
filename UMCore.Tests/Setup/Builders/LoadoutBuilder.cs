namespace UMCore.Tests.Setup.Builders;

public class LoadoutTemplateBuilder
{
    public static LoadoutTemplate Foo(string loadoutName="Foo")
    {
        return new()
        {
            Name = loadoutName,
            StartsWithSidekicks = true,
            Fighters = [
                new() {
                    Amount = 1,
                    IsHero = true,
                    IsRanged = false,
                    Key = "Foo",
                    MaxHealth = 10,
                    Movement = 2,
                    Name = "Foo",
                    Script = "function _Create() return UM.Build:Fighter():Build() end",
                    StartingHealth = 10,
                    Text = ""
                }
            ],
            Deck = [
                new() {
                    Amount = 5,
                    Card = CardTemplateBuilder.DefaultScheme()
                },
            ]
        };
    }
}