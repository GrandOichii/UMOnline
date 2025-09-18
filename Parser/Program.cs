using System.Text.Json;
using ScriptParser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var card = new Card {
    Name = "Test card",
    // Text = "After combat: Draw 2 cards. Gain 1 action.",
    Text = "After combat: Recover 2 health. Draw up to 2 cards. Gain 1 action.",
};


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

var numericSelector = new Selector()
{
    Name = "numericSelector",
    Children = [
        upToNumber,
        staticNumber
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
        drawCards,
        gainActions
    ]
};

var effectSplitter = new Splitter()
{
    Name = "effectSplitter",
    PatternString = "\\. ",
    // Script = File.ReadAllText("../scripts/effectSplitter.lua"),
    Children = {
        effectSelector
    }
};

var afterCombat = new Matcher() {
    Name = "afterCombat",
    PatternString = "After combat: (.+)",
    Script = File.ReadAllText("../scripts/afterCombat.lua"),
    Children = {
        effectSplitter
    }
};

var parser = new Matcher(){
    Name = "root",
    PatternString = "(.+)",
    Script = File.ReadAllText("../scripts/root.lua"),
    Children = {
        afterCombat
    }
};


var result = parser.Parse(card.Text);

var serializer = new SerializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();

try
{
    var serialized = serializer.Serialize(result);

    var script = result.CreateScript();

    File.WriteAllText("result.lua", script);

    File.WriteAllText("result.yaml", serialized);

}
catch (Exception e)
{
    System.Console.WriteLine(e);
    System.Console.WriteLine(e.StackTrace);
}