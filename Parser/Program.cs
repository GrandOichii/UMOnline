using System.Text.Json;
using ScriptParser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var TODOSCRIPT = File.ReadAllText("../scripts/todo.lua");
var RETURN_EMPTY_STRING_SCRIPT = "function _Create(text, children) return '--' end";
var returnTextScript = File.ReadAllText("../scripts/returnText.lua");
var FIGHTER_NAMES = new string[] {
    "Ms. Marvel",
    "Daredevil",
    "Medusa",
    "Sinbad",
    "Sherlock Holmes",
    "Buffy",
    "Hamlet",
    "Black Widow",
    "Angel",
    "Spike",
    "Alice",
    "Dr. Sattler",
    "Beowulf",
    "Robin Hood",
    "Dracula",
    "Bigfoot",
    "Achilles",
    "Jekyll & Hyde",
    "Titania",
    "Little Red",
    "Willow",
    "Luke Cage",
    "Bloody Mary",
    "Sun Wukong",
    "Black Panther",
    "The Wayward Sisters",
    "Deadpool",
    "Invisible Man",
    "Robert Muldoon",
    "Yennenga",
    "Bullseye",
    "Moon Knight",
    "Khonshu",
    "Mr. Knight",
    "Raptors",
    "Houdini",
    "Squirrel Girl",
    "Squirrels",
    "Ghost Rider",
    "Bruce Lee",
    "Ciri",
    "Ancient Leshen",
    "Eredin",
    "Philippa",
    "Leonardo",
    "Raphael",
    "Elektra",
    "Elektra Resurrected",
    "T. Rex",
    "Cloak",
    "Dagger",
    "The Genie",
    "Winter Soldier",
    "Nikola Tesla",
    "William Shakespeare",
    "Dr. Jill Trent",
    "Golden Bat",
    "Annie Christmas",
    "Spider-Man",
    "She-Hulk",
    "Doctor Strange",
    "Tomoe Gozen",
    "Oda Nobunaga",
    "Geralt of Rivia",
    "Yennefer",
    "King Arthur",
    "Shredder",
    "Krang",
    "Donatello",
    "Michelangelo"
};

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

var namedFighter = new Matcher()
{
    Name = "namedFighter",
    PatternString = $"({string.Join('|', FIGHTER_NAMES.Select(FormattedName))})\\.?",
    Script = "function _Create(text, children, data) return string.format(':Named(\\'%s\\')', data[2]:gsub('{DOTSPACE}', '. ')) end"
};

var fighterSelector = new Selector()
{
    Name = "fighterSelector",
    Children = [
        new Matcher() {
            Name = "thisFighter",
            PatternString = "this fighter",
            Script = "function _Create() return ':Only(UM.Fighters:Source())' end"
        },
        new Matcher() {
            Name = "allFighters",
            PatternString = "fighters?\\.?",
            Script = RETURN_EMPTY_STRING_SCRIPT,
        },

        // TODO these two can have suffixes: "opposing fighters adjacent to Dracula"
        new Matcher() {
            Name = "yourFighter",
            PatternString = "your fighters?\\.?",
            Script = "function _Create(text, children) return ':OwnedBy(UM.Players:EffectOwner())' end"
        },
        new Matcher() {
            Name = "opposingFighter",
            PatternString = "opposing fighters?\\.?",
            Script = "function _Create(text, children) return ':OpposingTo(UM.Players:EffectOwner())' end"
        },
        namedFighter,
    ]
};

var singleFighter = new Selector()
{
    Name = "singleFighter",
    Script = File.ReadAllText("../scripts/singleFighter.lua"),
    Children = [fighterSelector]
};

var eachFighter = new Matcher()
{
    Name = "eachFighter",
    Script = File.ReadAllText("../scripts/eachFighter.lua"),
    PatternString = "(?:each|all)(?: of)? (.+)",
    Children = [fighterSelector]
};

var fighter = new Selector()
{
    Name = "fighter",
    Script = File.ReadAllText("../scripts/fighter.lua"),
    Children = [
        eachFighter,
        singleFighter,
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
        fighter,
        numericSelector,
    ]
};

var spaceSelector = new Selector()
{
    Name = "spaceSelector",
    Script = File.ReadAllText("../scripts/spaceSelector.lua"),
    Children = [
        new Matcher() {
            Name = "anySpace",
            PatternString = "any space\\.?",
            Script = RETURN_EMPTY_STRING_SCRIPT,
        },
        new Matcher() {
            Name = "anyEmptySpace",
            PatternString = "any empty space\\.?",
            Script = "function _Create() return :Empty() end",
        },
        // new Matcher() { // TODO his|her|their refers to the target, not the source
        //     Name = "anySpaceInSameZone",
        //     PatternString = "any space in (?:her|his|their) zone\\.?",
        //     Script = File.ReadAllText("../scripts/anySpaceInSameZone.lua")
        // }
    ]
};

var place = new Matcher()
{
    Name = "place",
    PatternString = "Place (.+?) in (.+)",
    Script = File.ReadAllText("../scripts/place.lua"),
    Children = [
        fighter,
        spaceSelector,
    ]
};

var dealDamage = new Matcher()
{
    Name = "dealDamage",
    PatternString = "Deal (.+) damage to (.+)\\.?",
    Script = File.ReadAllText("../scripts/dealDamage.lua"),
    Children = [
        numericSelector,
        fighter,
    ]
};

var playerSelector = new Selector()
{
    Name = "playerSelector",
    Children = [
        new Matcher() {
            Name = "yourOpponent",
            PatternString = "[Y|y]our opponent ?",
            Script = "function _Create(text, children) return 'UM.Players:Opponent()' end"
        }
    ]
};

var discardEffect = new Matcher()
{
    Name = "discardEffect",
    PatternString = "(.+? )[D|d]iscards? (.+?)( random)? cards?\\.?",
    Script = "function _Create(text, children) return string.format('UM.Effects:Discard(\\n%s,\\n%s,\\n%s\\n)', children[1], children[2], children[3]) end",
    Children = [
        playerSelector,
        numericSelector,
        new Selector() {
            Name = "discardRandomCardsSelector",
            Children = [
                new Matcher() {
                    Name = "discardRandomCardsSelector_false",
                    PatternString = "",
                    Script = "function _Create() return 'false' end"
                },
                new Matcher() {
                    Name = "discardRandomCardsSelector_true",
                    PatternString = " random",
                    Script = "function _Create() return 'true' end"
                }
            ]
        }
    ]
};

// TODO not tested
var replaceCardValue = new Matcher()
{
    Name = "replaceCardValue",
    PatternString = "(?:this card's value|the value of this card) is ([0-9]+) instead\\.?",
    Script = "function _Create(text, children) return string.format('UM.Effects:ReplaceCardValue(\\n%s\\n)', children[1]) end",
    Children = [
        staticNumber,
    ]
};

// var recoverHealth = new Matcher()
// {
//     Name = "recoverHealth",
//     PatternString = "(.+? )?[R|r]ecovers? (.+) health\\.?",
//     Script = File.ReadAllText("../scripts/recover")
// };


var cancelOpponentsCardEffects = new Matcher()
{
    Name = "cancelOpponentsCardEffects",
    PatternString = "[C|c]ancel all effects on your opponent's card\\.?",
    Script = "function _Create(text, children) return 'UM.Effects:CancelAllEffectsOfOpponentsCard()' end"
};

var effectSelector = new Selector()
{
    Name = "effectSelector",
    Children = [
        changeSize,
        gainActions,
        drawCards,
        moveFighter,
        place,
        dealDamage,
        replaceCardValue,
        cancelOpponentsCardEffects,
        discardEffect,
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

var empty = new Matcher()
{
    Name = "empty",
    PatternString = "",
    Script = RETURN_EMPTY_STRING_SCRIPT
};

var rootSelector = new Selector()
{
    Name = "rootSelector",
    Children = [
        afterCombat,
        immediately,
        duringCombat,
        new Selector() {
            Name = "effectMatcher",
            Script = "function _Create(text, children) return string.format(':Effect(\\n\\'%s\\',\\n%s\\n)', text:gsub(\"'\", \"\\\\'\"):gsub('{DOTSPACE}', '. '), children[1]) end",
            Children = [
                effectSplitter,
            ]
        },
        empty,
    ]
};

var parser = new Matcher()
{
    Name = "root",
    PatternString = "(.*)",
    Script = File.ReadAllText("../scripts/root.lua"),
    Children = {
        rootSelector
    }
};

var cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("../cards.json"));
// List<Card> cards = [new Card {
//     Name = "Test card",
//     Text = "After combat: Place this fighter in any space.",
// }];

string FormattedName(string name)
{
    return name.Replace(". ", "{DOTSPACE}");
}

string TrasnformText(string text)
{
    foreach (var name in FIGHTER_NAMES)
    {
        text = text.Replace(name, FormattedName(name));
    }
    return text;
}


var analysis = new ParseResultAnalyzer();

var successCount = 0;

foreach (var card in cards!)
{

    var result = parser.Parse(TrasnformText(card.Text));

    analysis.Analyze(result);

    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    try
    {
        var serialized = serializer.Serialize(result);

        var script = result.CreateScript();

        var name = card.Name.Replace('?', ' ').Replace('!', ' ').Replace(':', ' ').Replace('"', ' ').Trim();
        if (result.Status == ParseResultStatus.SUCCESS)
        {
            System.Console.WriteLine($"Generated script for {card.Name}");
            File.WriteAllText($"../card-scripts/{name}.lua", script);
            ++successCount;
        }

        File.WriteAllText($"../reports/{name}.yaml", serialized);

    }
    catch (Exception e)
    {
        System.Console.WriteLine(e);
        System.Console.WriteLine(e.StackTrace);
        System.Console.WriteLine(e.InnerException);
        System.Console.WriteLine(e.InnerException?.StackTrace);
    }
}

File.WriteAllText("analysis.json", analysis.ToJson());
System.Console.WriteLine($"{successCount}/{cards.Count}");