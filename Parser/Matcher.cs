using System.Text.RegularExpressions;
using NLua;
using YamlDotNet.Serialization;

namespace ScriptParser;

public class Matcher : ParserBase {
    public string PatternString { 
        get => Pattern.ToString();
        set {
            Pattern = new Regex("^(?i)" + value + "$", RegexOptions.Multiline);
        }
    }
    public Regex Pattern { get; private set; } = null!;

    public int GroupCount => Pattern.GetGroupNames().Length - 1;

    public override ParseResult Parse(string text) {
        // TODO? check group count
        var match = Pattern.Match(text);
        if (!match.Success) return new MatcherParseResult(match.Groups, this, ParseResultStatus.DIDNT_MATCH, text);
        var status = ParseResultStatus.SUCCESS;
        var children = new List<ParseResult>();
        if (Children.Count == 0) return new MatcherParseResult(match.Groups, this, ParseResultStatus.SUCCESS, text);
        for (int i = 1; i < match.Groups.Count; i++) {
            var child = Children[i - 1];
            var group = match.Groups[i];
            var childResult = child.Parse(group.ToString());
            children.Add(childResult);
            // TODO
            if (childResult.Status != ParseResultStatus.SUCCESS)
                status = ParseResultStatus.CHILD_FAILED;
        }
        return new MatcherParseResult(match.Groups, this, status, text, children);
    }

    public bool GroupCountMatches() => GroupCount == Children.Count;

    public override string ParserType() => "matcher";
}

public class MatcherParseResult : ParseResult
{
    [YamlIgnore]
    public GroupCollection Groups { get; }

    public MatcherParseResult(GroupCollection groups, ParserBase parent, ParseResultStatus status, string text) : base(parent, status, text)
    {
        Groups = groups;
    }

    public MatcherParseResult(GroupCollection groups, ParserBase parent, ParseResultStatus status, string text, List<ParseResult> children) : base(parent, status, text, children)
    {
        Groups = groups;
    }

    private LuaTable GetData(Lua state) {
        var data = new List<string>();
        foreach (var group in Groups) {
            data.Add(group.ToString()!);
        }
        return LuaUtility.CreateTable(state, data);
    }

    public override object[] CallCreationFunc(Lua state, LuaFunction func, string text, LuaTable childrenTable) => func.Call(text, childrenTable, GetData(state));
}