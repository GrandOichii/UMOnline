namespace UMCore.Tests.Setup.Builders;

public class LoadoutCardTemplateBuilder
{
    private static int _buildCount = 0;

    private int _amount = 1;
    private List<string> _allowedFighters = [];
    private int _boost = 1;
    private string _key = "";
    private List<string> _labels = [];
    private string _name = "";
    private string _script = "";
    private string _text = "";
    private string _type = "";
    private int _value = 0;

    public LoadoutCardTemplateBuilder Amount(int amount)
    {
        _amount = amount;

        return this;
    }

    public LoadoutCardTemplateBuilder CanBePlayedByAny()
    {
        _allowedFighters = [];
        return this;
    }

    public LoadoutCardTemplateBuilder CanBePlayedBy(string name)
    {
        _allowedFighters.Add(name);
        return this;
    }

    public LoadoutCardTemplateBuilder HasLabel(string label)
    {
        _labels.Add(label);
        return this;
    }
    
    public LoadoutCardTemplateBuilder Value(int value)
    {
        _value = value;

        return this;
    }

    public LoadoutCardTemplateBuilder Boost(int boost)
    {
        _boost = boost;

        return this;
    }

    public LoadoutCardTemplateBuilder Script(string script)
    {
        _script = script;

        return this;
    }

    public LoadoutCardTemplateBuilder Versatile()
    {
        _type = "Versatile";

        return this;
    }
    
    public LoadoutCardTemplateBuilder Scheme()
    {
        _type = "Scheme";

        return this;
    }

    public LoadoutCardTemplateBuilder Name(string name)
    {
        _name = name;

        return this;
    }

    public LoadoutCardTemplateBuilder Feint()
    {
        _script = """
        :Immediately(
        'Immediately: Cancel all effects on your opponent\'s card.',
        UM.Effects:CancelAllEffectsOnOpponentsCard()
        )
        """;

        return this;
    }

    public LoadoutCardTemplate Build()
    {
        return new()
        {
            Amount = _amount,
            Card = new()
            {
                AllowedFighters = _allowedFighters,
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
            }
        };
    }
}