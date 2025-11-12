namespace UMCore.Tests.Setup.Builders;

public class CardTemplateBuilder
{
    private static int _buildCount = 0;

    private int _amount = 1;
    private List<string> _allowedFighters = [];
    private int? _boost = 1;
    private string _key = "";
    private List<string> _labels = [];
    private string _name = "";
    private string _script = "";
    private string _text = "";
    private string _type = "";
    private int _value = 0;

    public CardTemplateBuilder NoBoost()
    {
        _boost = null;
        return this;
    }

    public CardTemplateBuilder Amount(int amount)
    {
        _amount = amount;

        return this;
    }

    public CardTemplateBuilder CanBePlayedByAny()
    {
        _allowedFighters = [];
        return this;
    }

    public CardTemplateBuilder CanBePlayedBy(string name)
    {
        _allowedFighters.Add(name);
        return this;
    }

    public CardTemplateBuilder HasLabel(string label)
    {
        _labels.Add(label);
        return this;
    }
    
    public CardTemplateBuilder Value(int value)
    {
        _value = value;

        return this;
    }

    public CardTemplateBuilder Boost(int boost)
    {
        _boost = boost;

        return this;
    }

    public CardTemplateBuilder Script(string script)
    {
        _script = script;

        return this;
    }

    public CardTemplateBuilder Versatile()
    {
        _type = "Versatile";

        return this;
    }
    
    public CardTemplateBuilder Scheme()
    {
        _type = "Scheme";

        return this;
    }

    public CardTemplateBuilder Name(string name)
    {
        _name = name;

        return this;
    }

    public CardTemplateBuilder Feint()
    {
        _script = """
        :Immediately(
        'Immediately: Cancel all effects on your opponent\'s card.',
        UM.Effects:CancelAllEffectsOnOpponentsCard()
        )
        """;

        return this;
    }

    public CardTemplate Build()
    {
        return new()
        {
            Amount = _amount,
            AllowedFighters = [.. _allowedFighters],
            Boost = _boost,
            Key = string.IsNullOrEmpty(_key) ? $"lctb{_buildCount++}" : _key,
            Labels = [.. _labels],
            Name = _name,
            Text = _text,
            Script = $"""
            function _Create()
            return UM.Build:Card()
            {_script}
            :Build()
            end
            """,
            Type = _type,
            Value = _value,
            IncludedInDeckWithSidekick = null,
        };
    }
}