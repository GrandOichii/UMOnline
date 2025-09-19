using System.Text.Json;
using ScriptParser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("../cards.json"));

var todo = new Matcher()
{
    Name = "TODO",
    PatternString = ".+",
    Script = "function _Create(text, children) return 'TODO' end"
};

var staticNumber = new Matcher()
{
    Name = "staticNumber",
    PatternString = "[0-9]",
    Script = File.ReadAllText("../scripts/staticNumber.lua"),
};

var upToNumber = new Matcher()
{
    Name = "upToNumber",
    PatternString = "up to ([0-9])",
    Script = File.ReadAllText("../scripts/upToNumber.lua")
};

var singleNumber = new Matcher()
{
    Name = "singleNumber",
    PatternString = "an?",
    Script = "function _Create(text, children, data)return '1'end"
};

var numericSelector = new Selector()
{
    Name = "numericSelector",
    Children = [
        singleNumber,
        staticNumber,
        upToNumber,
    ]
};

var drawCards = new Matcher()
{
    Name = "drawCards",
    PatternString = "Draw (.+) cards?\\.?",
    Script = File.ReadAllText("../scripts/drawCards.lua"),
    Children = [
        numericSelector
    ]
};

var gainActions = new Matcher()
{
    Name = "gainActions",
    PatternString = "Gain ([0-9]+) actions?\\.?",
    Script = File.ReadAllText("../scripts/gainActions.lua"),
    Children = [
        staticNumber
    ]
};

var effectSelector = new Selector()
{
    Name = "effectSelector",
    Children = [
        gainActions,
        drawCards,
    ]
};

var youWonCondition = new Matcher()
{
    Name = "youWonCondition",
    PatternString = "you won the combat",
    Script = "function _Create(text, children, data)return 'UM.Conditional:CombatWonBy(\\nUM.Players:EffectOwner()\\n)' end"
};

var conditionalSelector = new Selector()
{
    Name = "conditionalSelector",
    Children = [
        youWonCondition
    ]
};

var ifMatcher = new Matcher()
{
    Name = "ifMatcher",
    PatternString = "If (.+), (.+)\\.?",
    Script = File.ReadAllText("../scripts/ifMatcher.lua"),
    Children = [
        conditionalSelector,
        effectSelector,
    ]
};

effectSelector.Children.Add(ifMatcher);

var effectSplitter = new Splitter()
{
    Name = "effectSplitter",
    PatternString = "\\. ",
    // Script = File.ReadAllText("../scripts/effectSplitter.lua"),
    Children = {
        effectSelector
    }
};

var afterCombat = new Matcher()
{
    Name = "afterCombat",
    PatternString = "After combat: (.+)",
    Script = File.ReadAllText("../scripts/afterCombat.lua"),
    Children = {
        effectSplitter
    }
};

var rootSelector = new Selector()
{
    Name = "rootSelector",
    Children = [
        afterCombat,
    ]
};

var parser = new Matcher()
{
    Name = "root",
    PatternString = "(.+)",
    Script = File.ReadAllText("../scripts/root.lua"),
    Children = {
        rootSelector
    }
};

// var card = new Card {
//     Name = "Test card",
//     Text = "Immediately: Cancel all effects on your opponent\u0027s card.",
//     // Text = "After combat: Recover 2 health. Draw up to 2 cards. Gain 1 action.",
// };

var analysis = new ParseResultAnalyzer();

var successCount = 0;

foreach (var card in cards!)
{

    var result = parser.Parse(card.Text);

    analysis.Analyze(result);

    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    try
    {
        var serialized = serializer.Serialize(result);

        var script = result.CreateScript();

        if (result.Status == ParseResultStatus.SUCCESS)
        {
            File.WriteAllText($"../cards/{card.Name}.lua", script);
            ++successCount;
        }

        File.WriteAllText($"../reports/{card.Name}.yaml", serialized);

    }
    catch (Exception e)
    {
        System.Console.WriteLine(e);
        System.Console.WriteLine(e.StackTrace);
    }
}

File.WriteAllText("analysis.json", analysis.ToJson());
System.Console.WriteLine($"{successCount}/{cards.Count}");