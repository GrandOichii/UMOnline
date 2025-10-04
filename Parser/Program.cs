using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using Parser;
using Parser.Parsers;

ParserBase ReadXMLParser()
{
    var doc = "../parser.drawio";
    XmlDocument xDoc = new();
    xDoc.Load(doc);

    var docRoot = xDoc.DocumentElement!;
    var root = docRoot.GetElementsByTagName("root")[0]!;

    var children = root
        .ChildNodes
        .OfType<XmlElement>();

    var mapping = new Dictionary<string, XmlElement>();

    var rootNodes = children
        .Where(n => n.GetAttribute("style").StartsWith("rhombus"))
        .ToList();
    // System.Console.WriteLine($"Found {rootNodes.Count} roots");

    var nodeNodes = children
        .Where(n => n.GetAttribute("style").StartsWith("swimlane"))
        .ToList();
    // System.Console.WriteLine($"Found {nodeNodes.Count} nodes");

    var connectorNodes = children
        .Where(n => n.GetAttribute("style").StartsWith("endArrow=classic"))
        .ToList();
    // System.Console.WriteLine($"Found {connectorNodes.Count} connectors");

    var referenceNodes = children
        .Where(n => n.GetAttribute("style").StartsWith("ellipse"))
        .ToList();
    // System.Console.WriteLine($"Found {referenceNodes.Count} references");

    var textNodes = children
        .Where(n => n.GetAttribute("style").StartsWith("text;strokeColor=none"))
        .ToList();
    // System.Console.WriteLine($"Found {textNodes.Count} texts");

    // create roots
    Dictionary<string, XmlParserRoot> roots = [];
    Dictionary<string, XmlParserRoot> idToRoot = [];
    foreach (var rootNode in rootNodes)
    {
        var id = rootNode.GetAttribute("id");
        var newRoot = new XmlParserRoot()
        {
            Name = rootNode.GetAttribute("value")
        };
        if (newRoot.Name.StartsWith("<span"))
        {
            throw new Exception($"{newRoot.Name} is not a valid root name");
        }
        roots.Add(newRoot.Name, newRoot);
        idToRoot.Add(id, newRoot);
    }

    // create nodes
    Dictionary<string, XmlParserNode> idToNode = [];
    foreach (var nodeNode in nodeNodes)
    {
        var id = nodeNode.GetAttribute("id");
        var newNode = new XmlParserNode()
        {
            Name = nodeNode.GetAttribute("value")
        };
        if (newNode.Name.StartsWith("<span"))
        {
            throw new Exception($"{newNode.Name} is not a valid node name");
        }

        idToNode.Add(id, newNode);

    }

    // connect node texts
    foreach (var textNode in textNodes)
    {
        var parentId = textNode.GetAttribute("parent");
        var parent = idToNode[parentId];
        var newLine = new XmlParserNodeLine()
        {
            Text = textNode.GetAttribute("value")
        };
        if (newLine.Text.StartsWith("<span"))
        {
            throw new Exception($"{newLine.Text} is not a valid node line");
        }
        parent.Lines.Add(newLine);
    }

    // check references
    var idToRef = new Dictionary<string, XmlParserNode>();
    foreach (var refNode in referenceNodes)
    {
        var newRef = new XmlParserNode()
        {
            Name = refNode.GetAttribute("value"),
            IsReference = true,
        };
        if (newRef.Name.StartsWith("<span"))
        {
            throw new Exception($"{newRef.Name} is not a valid reference");
        }
        if (!roots.ContainsKey(newRef.Name))
        {
            throw new Exception($"Found reference to {newRef.Name}, which is not defined");
        }
        idToRef.Add(refNode.GetAttribute("id"), newRef);
    }

    // create templates
    // var templates = new Dictionary<string, ParserBase>();
    foreach (var connectorNode in connectorNodes)
    {
        var srcId = connectorNode.GetAttribute("source");
        var tgtId = connectorNode.GetAttribute("target");
        var idx = connectorNode.GetAttribute("value");
        // root -> node
        if (idToRoot.ContainsKey(srcId))
        {
            var r = idToRoot[srcId];
            if (r.Node is not null)
            {
                throw new Exception($"Provided multiple node connections for root {r.Name}");
            }
            // TODO idx can be empty
            // System.Console.WriteLine($"(ROOT){idToRoot[srcId].Name} -> (NODE){idToNode[tgtId].Name} [{idx}]");
            r.Node = idToNode[tgtId];

            continue;
        }
        // node -> reference
        if (idToRef.ContainsKey(tgtId))
        {
            // TODO idx cant be empty
            // System.Console.WriteLine($"(NODE){idToNode[srcId].Name} -> (REF){idToRef[tgtId].Name} p{idx}]");
            idToNode[srcId].AddChild(idToRef[tgtId], idx);
            continue;
        }

        // TODO idx cant be empty
        // System.Console.WriteLine($"(NODE){idToNode[srcId].Name} -> (NODE){idToNode[tgtId].Name} [{idx}]");
        idToNode[srcId].AddChild(idToNode[tgtId], idx);
    }

    // foreach (var node in idToNode.Values)
    // {
    //     System.Console.WriteLine($"Node {node.Name}: {node.Children.Count}");
    //     foreach (var line in node.Lines)
    //         System.Console.WriteLine($"\t{line.Text}");
    // }

    Dictionary<XmlParserRoot, ParserBase> parserRoots = [];
    Dictionary<string, ParserBase> nameToParser = [];
    foreach (var xmlRoot in roots.Values)
    {
        // System.Console.WriteLine(xmlRoot.Name);
        if (xmlRoot.Node is null)
        {
            throw new Exception($"No node connected to root {xmlRoot.Name}");
        }
        var parserNode = xmlRoot.Node.ToParser();
        parserRoots.Add(xmlRoot, parserNode);
        nameToParser.Add(xmlRoot.Name, parserNode);
    }

    foreach (var pair in parserRoots)
    {
        var xml = pair.Key;
        var parser = pair.Value;
        foreach (var child in xml.Node!.GetChildren())
        {
            var parserNode = child.ToParserRecursive(nameToParser);
            parser.Children.Add(parserNode);
        }
    }

    if (!nameToParser.TryGetValue("root", out ParserBase? result))
    {
        throw new Exception("Failed to find root");
    }

    return result;

}


// Selector TestSelector(string name)
// {
//     return new Selector()
//     {
//         Name = name,
//         Children = []
//     };
// }

string ScriptFile(string name)
{
    return File.ReadAllText($"../scripts/{name}.lua");
}

var TODOSCRIPT = ScriptFile("todo");
// var RETURN_EMPTY_STRING_SCRIPT = "function _Create(text, children) return '--' end";
var returnTextScript = ScriptFile("returnText");
var FIGHTER_NAMES = new string[] {
    "Ms. Marvel",
    "Daredevil",
    "Annie",
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
    "Holmes",
    "Bigfoot",
    "Shuri",
    "the Jackalope",
    "T-Rex",
    "Achilles",
    "Jekyll & Hyde",
    "Dr. Jekyll",
    "Mr. Hyde",
    "Titania",
    "Little Red",
    "Willow",
    "Luke Cage",
    "Bloody Mary",
    "Sun Wukong",
    "Black Panther",
    "The Wayward Sisters",
    "Dr. Malcolm",
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
    "Squirrel",
    "Annie Christmas",
    "Spider-Man",
    "She-Hulk",
    "Doctor Strange",
    "Tomoe Gozen",
    "Oda Nobunaga",
    "Geralt of Rivia",
    "Yennefer",
    "King Arthur",
    "Shakespeare",
    "Oberon",
    "the Genie",
    "Shredder",
    "Harpy",
    "Krang",
    "Ihuarraquax",
    "Faith",
    "Donatello",
    "Dr. Watson",
    "Dr. Strange",
    "Michelangelo"
};

// var todo = new Matcher()
// {
//     Name = "TODO",
//     PatternString = ".+",
//     Script = "function _Create(text, children) return 'TODO' end"
// };

// var empty = new Matcher()
// {
//     Name = "empty",
//     PatternString = "",
//     Script = RETURN_EMPTY_STRING_SCRIPT
// };

// var changeSize = new Matcher()
// {
//     Name = "changeSize",
//     PatternString = "[C|c]hange size\\.?",
//     Script = "function _Create(text, children) return 'UM.Effects:ChangeSize()' end",
//     Children = [],
// };

// var staticNumber = new Matcher()
// {
//     Name = "staticNumber",
//     PatternString = "[0-9]",
//     Script = ScriptFile("staticNumber"),
// };

// var upToNumber = new Matcher()
// {
//     Name = "upToNumber",
//     PatternString = "up to ([0-9])",
//     Script = ScriptFile("upToNumber")
// };

// var singleNumber = new Matcher()
// {
//     Name = "singleNumber",
//     PatternString = "an?",
//     Script = "function _Create(text, children, data)return '1'end"
// };

// var numericSelector = new Selector()
// {
//     Name = "numericSelector",
//     Children = [
//         singleNumber,
//         staticNumber,
//         upToNumber,
//     ]
// };

// var youDrawCards = new Matcher()
// {
//     Name = "youDrawCards",
//     PatternString = "(?:[Y|y]ou )?[D|d]raw (.+) cards?\\.?",
//     Script = ScriptFile("youDrawCards"),
//     Children = [
//         numericSelector
//     ]
// };

// var yourOpponentDrawCards = new Matcher()
// {
//     Name = "yourOpponentDrawCards",
//     PatternString = "[Y|y]our opponent draws? (.+) cards?\\.?",
//     Script = ScriptFile("yourOpponentDrawCards"),
//     Children = [
//         numericSelector
//     ]
// };

// var gainActions = new Matcher()
// {
//     Name = "gainActions",
//     PatternString = "[G|g]ain ([0-9]+) actions?\\.?",
//     Script = ScriptFile("gainActions"),
//     Children = [
//         staticNumber
//     ]
// };

// var namedFighter = new Matcher()
// {
//     Name = "namedFighter",
//     PatternString = $"({string.Join('|', FIGHTER_NAMES.Select(FormattedName))})\\.?",
//     Script = "function _Create(text, children, data) return string.format(':Named(\\'%s\\')', data[2]:gsub('{DOTSPACE}', '. ')) end"
// };

// var fighterSelector = new Selector()
// {
//     Name = "fighterSelector",
//     Children = [
//         new Matcher() {
//             Name = "adjacentToNamed",
//             PatternString = "fighter adjacent to (.+?)\\.?",
//             Script = "function _Create(text, children) return string.format(':AdjacentTo(\\nUM.S:Fighters()\\n%s\\n:BuildOne()\\n)', children[1]) end",
//             Children = [
//                 namedFighter
//             ]
//         },
//         new Matcher() {
//             Name = "thisFighter",
//             PatternString = "this fighter",
//             Script = "function _Create() return ':Only(UM.Fighters:Source())' end"
//         },
//         new Matcher() {
//             Name = "allFighters",
//             PatternString = "fighters\\.?",
//             Script = RETURN_EMPTY_STRING_SCRIPT,
//         },

//         // TODO these two can have suffixes: "opposing fighters adjacent to Dracula"
//         new Matcher() {
//             Name = "yourFighter",
//             PatternString = "[Y|y]our fighter\\.?",
//             Script = "function _Create(text, children) return ':Only(UM.Fighters:Source())' end"
//         },
//         // new Matcher() {
//         //     Name = "yourFighter",
//         //     PatternString = "[Y|y]our fighters\\.?",
//         //     Script = "function _Create(text, children) return ':OwnedBy(UM.Player:EffectOwner())' end"
//         // },
//         new Matcher() {
//             Name = "opposingFighter",
//             PatternString = "opposing fighters\\.?",
//             Script = "function _Create(text, children) return ':OpposingTo(UM.Player:EffectOwner())' end"
//         },
//         new Matcher() {
//             Name = "theOpposingFighter",
//             PatternString = "the opposing fighter\\.?",
//             Script = "function _Create() return ':OpposingInCombatTo(UM.Fighters:Source())' end"
//         },
//         new Matcher() {
//             Name = "anyFighterInNamedFighterZone",
//             PatternString = "any 1 fighter in (.+?)(?:\'s)? zone\\.?",
//             Script = "function _Create(text, children) return string.format(':InSameZoneAs(UM.S:Fighters()%s:BuildOne())', children[1]) end",
//             Children = [
//                 namedFighter
//             ]
//         },
//         namedFighter,
//     ]
// };

// var singleFighter = new Selector()
// {
//     Name = "singleFighter",
//     Script = ScriptFile("singleFighter"),
//     Children = [fighterSelector]
// };

// var eachFighter = new Matcher()
// {
//     Name = "eachFighter",
//     Script = ScriptFile("eachFighter"),
//     PatternString = "(?:each|all)(?: of)? (.+)",
//     Children = [fighterSelector]
// };

// var fighter = new Selector()
// {
//     Name = "fighter",
//     Script = ScriptFile("fighter"),
//     Children = [
//         eachFighter,
//         singleFighter,
//         new Matcher() {
//             Name = "anyFighter",
//             PatternString = "[A|a]ny fighter\\.?",
//             Script = "function _Create() return ':Single()' end"
//         },
//         // new Matcher() { // TODO finish this
//         //     Name = "adjacentFighter",
//         //     PatternString = "an adjacent fighter\\.?",
//         //     Script = "function _Create() return ':AdjacentTo(UM.Fighters:Source()):Single()' end"
//         //     // TODO could be wrong about effects like "Your fighter deals 1 damage to an adjacent fighter"
//         // }
//         // singleNamedFighter,
//         // ownedSingleFighter,
//         // eachFighter,
//     ]
// };

// var moveFighter = new Matcher()
// {
//     Name = "moveFighter",
//     PatternString = "[M|m]ove (.+) (up to .+) spaces?\\.?",
//     Script = ScriptFile("moveFighter"),
//     Children = [
//         fighter,
//         numericSelector,
//     ]
// };

// var spaceSelector = new Selector()
// {
//     Name = "spaceSelector",
//     Script = ScriptFile("spaceSelector"),
//     Children = [
//         new Matcher() {
//             Name = "anySpace",
//             PatternString = "any space\\.?",
//             Script = RETURN_EMPTY_STRING_SCRIPT,
//         },
//         new Matcher() {
//             Name = "anyEmptySpace",
//             PatternString = "any empty space\\.?",
//             Script = "function _Create() return :Empty() end",
//         },
//         new Matcher() { //? his|her|their refers to the target, not the source - isn't an issue I hope
//             Name = "anySpaceInSameZone",
//             PatternString = "any space in (?:her|his|their) zone\\.?",
//             Script = ScriptFile("anySpaceInSameZone")
//         }
//     ]
// };

// var place = new Matcher()
// {
//     Name = "place",
//     PatternString = "Place (.+?) in (.+)",
//     Script = ScriptFile("place"),
//     Children = [
//         fighter,
//         spaceSelector,
//     ]
// };

// var dealDamage = new Matcher()
// {
//     Name = "dealDamage",
//     PatternString = "[D|d]eal (.+) damage to (.+)\\.?",
//     Script = ScriptFile("dealDamage"),
//     Children = [
//         numericSelector,
//         fighter,
//     ]
// };

// var playerSelector = new Selector()
// {
//     Name = "playerSelector",
//     Children = [
//         new Matcher() {
//             Name = "yourOpponent",
//             PatternString = "[Y|y]our opponent ?",
//             Script = "function _Create(text, children) return 'UM.S:Players():OpposingTo(UM.Player:EffectOwner()):Single():Build()' end"
//         }
//     ]
// };

// var discardEffect = new Matcher()
// {
//     Name = "discardEffect",
//     PatternString = "(.+? )[D|d]iscards? (.+?)( random)? cards?\\.?",
//     Script = "function _Create(text, children) return string.format('UM.Effects:Discard(\\n%s,\\n%s,\\n%s\\n)', children[1], children[2], children[3]) end",
//     Children = [
//         playerSelector,
//         numericSelector,
//         new Selector() {
//             Name = "discardRandomCardsSelector",
//             Children = [
//                 new Matcher() {
//                     Name = "discardRandomCardsSelector_false",
//                     PatternString = "",
//                     Script = "function _Create() return 'false' end"
//                 },
//                 new Matcher() {
//                     Name = "discardRandomCardsSelector_true",
//                     PatternString = " random",
//                     Script = "function _Create() return 'true' end"
//                 }
//             ]
//         }
//     ]
// };

// // TODO not tested
// var replaceCardValue = new Matcher()
// {
//     Name = "replaceCardValue",
//     PatternString = "(?:this card's value|the value of this card) is ([0-9]+) instead\\.?",
//     Script = "function _Create(text, children) return string.format('UM.Effects:ReplaceCardValue(\\n%s\\n)', children[1]) end",
//     Children = [
//         staticNumber,
//     ]
// };

// var recoverHealth = new Matcher()
// {
//     Name = "recoverHealth",
//     PatternString = "(.+? )?[R|r]ecovers? (.+) health\\.?",
//     Script = ScriptFile("recoverHealth"),
//     Children = [
//         new Selector() {
//             Name = "recoverHealthTargetSelector",
//             Children = [
//                 new Matcher() {
//                     Name = "recoverEmptyTarget",
//                     PatternString = "",
//                     Script = "function _Create(text, children) return 'UM.S:Fighters():OwnedBy(UM.Player:EffectOwner()):Single():Build()' end"
//                 },
//                 new Matcher() {
//                     Name = "recoverFighterTarget",
//                     PatternString = "(.+) ",
//                     Script = "function _Create(text, children) return children[1] end",
//                     Children = [
//                         fighter
//                     ]
//                 }
//             ]
//         },
//         numericSelector
//     ]
// };

// // TODO finish this
// // var returnEffect = new Matcher()
// // {
// //     Name = "returnEffect",
// //     PatternString = "[R|r]eturn a defeated (.+) \\(if any\\) to (.+) in (.+)\\.?",
// //     Script = TODOSCRIPT,
// //     Children = [
// //         new Selector() {
// //             Name = "TestSelector1",
// //             Children = []
// //         },
// //         spaceSelector,
// //         new Selector() {
// //             Name = "TestSelector3",
// //             Children = []
// //         },
// //     ]
// // };

// var cancelOpponentsCardEffects = new Matcher()
// {
//     Name = "cancelOpponentsCardEffects",
//     PatternString = "[C|c]ancel all effects on your opponent's card\\.?",
//     Script = "function _Create(text, children) return 'UM.Effects:CancelAllEffectsOfOpponentsCard()' end"
// };

// var boostCard = new Matcher()
// {
//     Name = "boostCard",
//     PatternString = "[Y|y]ou may BOOST this (?:card|attack)( .+ times)?\\.?",
//     Script = "function _Create(text, children) return string.format('UM.Effects:AllowOptionalBoost(\\n%s\\n)', children[1]) end",
//     Children = [
//         new Selector() {
//             Name = "boostCardOptionalTimes",
//             Children = [
//                 new Matcher() {
//                     Name = "boostCard1Time",
//                     PatternString = "",
//                     Script = "function _Create() return 'UM:Static(1)' end"
//                 },
//                 new Matcher() {
//                     Name = "boostCardTimes",
//                     PatternString = " (.+) times",
//                     Script = "function _Create(text, children) return children[1] end",
//                     Children = [
//                         numericSelector
//                     ]
//                 },
//             ]
//         }
//     ]
// };

// var effectSelector = new Selector()
// {
//     Name = "effectSelector",
//     Children = [
//         changeSize,
//         gainActions,
//         youDrawCards,
//         yourOpponentDrawCards,
//         moveFighter,
//         recoverHealth,
//         // returnEffect,
//         place,
//         dealDamage,
//         replaceCardValue,
//         boostCard,
//         cancelOpponentsCardEffects,
//         discardEffect,
//         new Matcher() {
//             Name = "reminderText",
//             PatternString = @"\(.+\)",
//             Script = "function _Create(text, children, data) return '--' end"
//         }
//     ]
// };

// var startedTurnInCondition = new Matcher()
// {
//     Name = "startedTurnInCondition",
//     PatternString = @"(.+) started this turn [i|o]n a (.+)",
//     Script = ScriptFile("startedInCondition"),
//     Children = [
//         singleFighter,
//         new Selector() {
//             Name = "startedInSelector",
//             Children = [
//                 new Matcher() {
//                     Name = "differentSpace",
//                     PatternString = "different space",
//                     Script = TODOSCRIPT
//                 }
//             ]
//         }
//     ]
// };

// var youWonCondition = new Matcher()
// {
//     Name = "youWonCondition",
//     PatternString = "you won the combat",
//     Script = "function _Create(text, children, data)return 'UM.Conditional:CombatWonBy(\\nUM.Player:EffectOwner()\\n)' end"
// };

// var youLostCondition = new Matcher()
// {
//     Name = "youLostCondition",
//     PatternString = "you lost the combat",
//     Script = "function _Create(text, children, data)return 'UM.Conditional:CombatLostBy(\\nUM.Player:EffectOwner()\\n)' end"
// };

// var conditionalSelector = new Selector()
// {
//     Name = "conditionalSelector",
//     Children = [
//         youWonCondition,
//         youLostCondition,
//         startedTurnInCondition,
//     ]
// };

// var ifMatcher = new Matcher()
// {
//     Name = "ifMatcher",
//     PatternString = "If (.+), (.+)\\.?",
//     Script = ScriptFile("ifMatcher"),
//     Children = [
//         conditionalSelector,
//         effectSelector,
//     ]
// };

// effectSelector.Children.Add(ifMatcher);

// var effectSplitter = new Splitter()
// {
//     Name = "effectSplitter",
//     PatternString = @"\. |[T|t]hen, |, [T|t]hen ",
//     // Script = ScriptFile("effectSplitter"),
//     Children = {
//         effectSelector
//     }
// };

// var ifInstead = new Matcher()
// {
//     Name = "ifInstead",
//     PatternString = "(.+)\\. If (.+), (.+) instead\\.",
//     Script = ScriptFile("ifInstead"),
//     Children = [
//         effectSelector,
//         conditionalSelector,
//         effectSelector,
//     ]
// };

// var mayIf = new Matcher()
// {
//     Name = "mayIf",
//     PatternString = @"(.+?) may (.+?)\. If .+? do( not)?, (.+)",
//     Script = TODOSCRIPT,
//     Children = [
//         TestSelector("s1"),
//         TestSelector("s2"),
//         TestSelector("s3"),
//         TestSelector("s4"),
//     ]
// };

// var moveCanMoveThroughOpponents = new Matcher()
// {
//     // Move Bigfoot up to 5 spaces. You may move
//     Name = "moveCanMoveThroughOpponents",
//     PatternString = "[M|m]ove (.+) (up to .+) spaces?\\. .+ may move .+ through spaces containing opposing fighters\\.",
    
//     Script = ScriptFile("moveFighterCanMoveThroughOpponents"),
//     // Script = TODOSCRIPT,
//     Children = [
//         fighter,
//         numericSelector
//     ]

// };

// var effects = new Selector()
// {
//     Name = "effects",
//     Children = [
//         mayIf,
//         ifInstead,
//         moveCanMoveThroughOpponents,
//         effectSplitter
//     ]
// };

// var afterCombat = new Matcher()
// {
//     Name = "afterCombat",
//     PatternString = "After combat: (.+)",
//     Script = ScriptFile("afterCombat"),
//     Children = {
//         effects
//     }
// };

// var immediately = new Matcher()
// {
//     Name = "immediately",
//     PatternString = "Immediately: (.+)",
//     Script = ScriptFile("immediately"),
//     Children = {
//         effects
//     }
// };

// var duringCombat = new Matcher()
// {
//     Name = "duringCombat",
//     PatternString = "During combat: (.+)",
//     Script = ScriptFile("duringCombat"),
//     Children = {
//         effects
//     }
// };

// var rootSelector = new Selector()
// {
//     Name = "rootSelector",
//     Children = [
//         empty,
//         afterCombat,
//         immediately,
//         duringCombat,
//         new Selector() {
//             Name = "effectMatcher",
//             Script = "function _Create(text, children) return string.format(':Effect(\\n\\'%s\\',\\n%s\\n)', text:gsub(\"'\", \"\\\\'\"):gsub('{DOTSPACE}', '. '), children[1]) end",
//             Children = [
//                 effects,
//             ]
//         },
//     ]
// };

// var parser = new Matcher()
// {
//     Name = "root",
//     PatternString = "(.*)",
//     Script = ScriptFile("root"),
//     Children = {
//         rootSelector
//     }
// };

var parser = ReadXMLParser();

string FormattedName(string name)
{
    return name.Replace(". ", "{DOTSPACE}");
}

string FormatText(string text)
{
    foreach (var name in FIGHTER_NAMES)
    {
        text = text.Replace(name, FormattedName(name));
    }

    foreach (var (numS, num) in new List<(string, int)>{
        ("one", 1),
        ("two", 2),
        ("three", 3),
        ("four", 4),
        ("five", 5),
        ("six", 6),
        ("seven", 7),
        ("eight", 8),
        ("nine", 9),
    }) text = Regex.Replace(text, $"\\s{numS}\\s", $" {num} ");
    
    foreach (var (find, replace) in new List<(string, string)>{
        ("twice", "2 times"),
    }) text = text.Replace(find, replace);

    text = text
        .Replace("\n-", "-")
        .Replace(" (This includes opposing fighters.)", "")
        .Replace("(If you played this as your first action, end your turn.)", "")
        .Replace("(This is in addition to any boost from King Arthur's special ability.)", "")
        .Replace("(If a card does not have a BOOST value, it is treated as 0.)", "")
        .Replace("(You may do both.)", "")
        .Replace("(You do not spend Rage for this effect.)", "")
        .Replace("(\uD83C\uDF1F counts as any 1 $PELT$$ROSE$$SWORDS$ symbol.)", "")
        .Replace("(You do not spend Hellfire for this effect.)", "")
        .Replace("(This is in addition to any BLIND BOOST from Daredevil's special ability.)", "")
        .Replace("(Draw a card for Black Panther's special ability each time you BOOST.)", "")
        .Replace("(Do not discard any cards from your cauldron to cast these spells.)", "")
    ;

    return text;
}

var cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("../cards.json"));
// List<Card> cards = [new Card {
//     Name = "Test card",
//     Text = "Return your opponent\u0027s attack card to their hand. Look at their hand and choose an attack or versatile card for them to play. (It may be the same card.).",
// }];

var analysis = new ParseResultAnalyzer();

var successCount = 0;

foreach (var card in cards!)
{
    var result = parser.Parse(FormatText(card.Text));

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
            File.WriteAllText($"{args[2]}/{name}.lua", script);
            ++successCount;
        }

        File.WriteAllText($"{args[1]}/{name}.yaml", serialized);

    }
    catch (Exception e)
    {
        System.Console.WriteLine(e);
        System.Console.WriteLine(e.StackTrace);
        System.Console.WriteLine(e.InnerException);
        System.Console.WriteLine(e.InnerException?.StackTrace);
        System.Console.WriteLine(card.Name);
        return;
    }
}

File.WriteAllText("analysis.json", analysis.ToJson());
System.Console.WriteLine($"{successCount}/{cards.Count}");