using System.Text.Json;
using ScriptParser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var TODOSCRIPT = File.ReadAllText("../scripts/todo.lua");
var returnTextScript = File.ReadAllText("../scripts/returnText.lua");
var FIGHTER_NAMES = new string[] {
        "Black Widow",
        "Alice",
        "King Arthur",
        "Faith",
        "Holmes",
        "Sinbad",
        "Daredevil",
        "Bullseye",
        "Squirrel Girl",
        "Ciri",
        "Yennenga",
        "Ihuarraquax",
        "Harpy",
        "squirrel",
        "InGen Worker",
    };

var cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("../cards.json"));

var todo = new Matcher()
{
    Name = "TODO",
    PatternString = ".+",
    Script = "function _Create(text, children) return 'TODO' end"
};

var changeSize = new Matcher()
{
    Name = "changeSize",
    PatternString = "[C|c]hange size\\.?",
    Script = "function _Create(text, children) return 'UM.Effects:ChangeSize()' end",
    Children = [],
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
    PatternString = "[D|d]raw (.+) cards?\\.?",
    Script = File.ReadAllText("../scripts/drawCards.lua"),
    Children = [
        numericSelector
    ]
};

var gainActions = new Matcher()
{
    Name = "gainActions",
    PatternString = "[G|g]ain ([0-9]+) actions?\\.?",
    Script = File.ReadAllText("../scripts/gainActions.lua"),
    Children = [
        staticNumber
    ]
};

// var singleNamedFighter = new Matcher()
// {
//     Name = "singleNamedFighter",
//     PatternString = string.Join("|", FIGHTER_NAMES),
//     Script = File.ReadAllText("../scripts/singleNamedFighter.lua"),
//     Children = []
// };

// var ownedSingleFighter = new Matcher()
// {
//     Name = "ownedSingleFighter",
//     PatternString = "(.+) fighter",
//     Script = File.ReadAllText("../scripts/ownedSingleFighter.lua"),
//     Children = [
//         new Selector() {
//             Name = "ownedSingleFighterSelector",
//             Children = [
//                 new Matcher() {
//                     Name = "yourFighter",
//                     PatternString = "your",
//                     Script = "function _Create(text, children, data) return ':OwnedBy(UM.Players:EffectOwner())' end"
//                 },
//                 new Matcher() {
//                     Name = "opposingFighter",
//                     PatternString = "the opposing",
//                     Script = "function _Create(text, children, data) return ':OpposingTo(UM.Players:EffectOwner())' end"
//                 },
//                 new Matcher() {
//                     Name = "yourOtherFighter",
//                     PatternString = "your other",
//                     Script = "function _Create(text, children, data) return ':OwnedBy(UM.Players:EffectOwner())\\n:Except(UM.Fighters:Source())' end"
//                 },
//             ]
//         }
//     ]
// };

// var eachNamedFighter = new Matcher()
// {
//     Name = "eachNamedFighter",
//     PatternString = string.Join('|', FIGHTER_NAMES),
//     Script = "function _Create(text, children) return ':Named(\\''..text..'\\')' end",
//     Children = []
// };

// var eachFighter = new Matcher()
// {
//     Name = "eachFighter",
//     PatternString = "each (?:of )?(.+)",
//     Script = "function _Create(text, children) return string.format('UM.S.Fighters()\\n%s\\n:Build()', children[1])end",
//     Children = [
//         new Selector() {
//             Name = "TestSelector",
//             Children = [
//                 eachNamedFighter,
//                 new Matcher() {
//                     Name = "eachYourNamedFighter",
//                     PatternString = "your (.+)e?s", // TODO.
//                     Script = "function _Create(text, children) print(children[1]) return string.format(':OwnedBy(UM.Players:EffectOwner())\\n%s', children[1]) end",
//                     Children = [
//                         eachNamedFighter
//                     ]
//                 }
//             ]
//         }
//     ]
// };

var fighterSelector = new Selector()
{
    Name = "fighterSelector",
    Children = [
        // singleNamedFighter,
        // ownedSingleFighter,
        // eachFighter,
    ]
};

var moveFighter = new Matcher()
{
    Name = "moveFighter",
    PatternString = "Move (.+) (up to .+) spaces\\.?",
    Script = File.ReadAllText("../scripts/moveFighter.lua"),
    Children = [
        fighterSelector,
        numericSelector,
    ]
};

var dealDamage = new Matcher()
{
    Name = "dealDamage",
    PatternString = "Deal (.+) damage to (.+)\\.?",
    Script = File.ReadAllText("../scripts/dealDamage.lua"),
    Children = [
        numericSelector,
        fighterSelector,
    ]
};

var effectSelector = new Selector()
{
    Name = "effectSelector",
    Children = [
        changeSize,
        gainActions,
        drawCards,
        moveFighter,
        dealDamage,
    ]
};

var youWonCondition = new Matcher()
{
    Name = "youWonCondition",
    PatternString = "you won the combat",
    Script = "function _Create(text, children, data)return 'UM.Conditional:CombatWonBy(\\nUM.Players:EffectOwner()\\n)' end"
};

var youLostCondition = new Matcher()
{
    Name = "youLostCondition",
    PatternString = "you lost the combat",
    Script = "function _Create(text, children, data)return 'UM.Conditional:CombatLostBy(\\nUM.Players:EffectOwner()\\n)' end"
};

var conditionalSelector = new Selector()
{
    Name = "conditionalSelector",
    Children = [
        youWonCondition,
        youLostCondition,
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

var immediately = new Matcher()
{
    Name = "immediately",
    PatternString = "Immediately: (.+)",
    Script = File.ReadAllText("../scripts/immediately.lua"),
    Children = {
        effectSplitter
    }
};

var duringCombat = new Matcher()
{
    Name = "duringCombat",
    PatternString = "During combat: (.+)",
    Script = File.ReadAllText("../scripts/duringCombat.lua"),
    Children = {
        effectSplitter
    }
};

var rootSelector = new Selector()
{
    Name = "rootSelector",
    Children = [
        afterCombat,
        immediately,
        duringCombat,
        new Matcher() {
            Name = "effectMatcher",
            PatternString = "(.+)",
            Script = "function _Create(text, children) return string.format(':Effect(\\n%s\\n)', children[1]) end",
            Children = [
                effectSplitter,
            ]
        }
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

        var name = card.Name.Replace('?', ' ').Replace('!', ' ').Replace('"', ' ').Trim();
        if (result.Status == ParseResultStatus.SUCCESS)
        {
            System.Console.WriteLine($"Generated script for {card.Name}");
            File.WriteAllText($"../cards/{name}.lua", script);
            ++successCount;
        }

        File.WriteAllText($"../reports/{name}.yaml", serialized);

    }
    catch (Exception e)
    {
        System.Console.WriteLine(e);
        System.Console.WriteLine(e.StackTrace);
    }
}

File.WriteAllText("analysis.json", analysis.ToJson());
System.Console.WriteLine($"{successCount}/{cards.Count}");