namespace UMCore.Tests.Setup.Builders;

public class CardTemplateBuilder
{
    public static CardTemplate DefaultScheme()
    {
        return new()
        {
            AllowedFighters = [],
            Boost = 1,
            Key = "scheme",
            Name = "Scheme",
            Script = "function _Create() return UM.Build.Card():Build() end",
            Text = "",
            Type = "Scheme",
            Value = 0,
            Labels = [],
            IncludedInDeckWithSidekick = null,
        };
    }
}