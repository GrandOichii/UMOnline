using NLua;
using YamlDotNet.Serialization;

namespace Parser.Parsers;

public enum ParseResultStatus
{
    SUCCESS = 0,
    DIDNT_MATCH,
    CHILD_FAILED,
    ALL_CHILDREN_FAILED,
    IGNORED,
}

public class ParseResult {
    [YamlIgnore]
    public ParserBase Parent { get; }
    // public readonly Parser Parent;
    [YamlMember(Alias = "name")]
    public string ParentName { get; }
    public ParseResultStatus Status { get; set; }
    public List<ParseResult> Children { get; }
    public string Text { get; }

    public ParseResult(ParserBase parent, ParseResultStatus status, string text) {
        Text = text;
        Parent = parent;
        ParentName = parent.Name;
        Status = status;
        Children = new();
    }

    public ParseResult(ParserBase parent, ParseResultStatus status, string text, List<ParseResult> children) : this(parent, status, text) {
        Children = children;
    }

    public override string ToString()
    {
        var result = $"{Parent.Name}\tStatus: {Status}";
        foreach (var child in Children) {
            result += $"\n[\n{child}\n]";
        }
        return result;
    }

    public string CreateScript() {
        var state = new Lua();
        return CreateScript(state);
    }

    public string CreateScript(Lua state, int indent = 0) {
        if (Status != ParseResultStatus.SUCCESS) return string.Empty;
        
        var children = new List<string>();
        foreach (var child in Children) {
            var s = child.CreateScript(state, indent + 1);
            // var split = s.Split("\n");
            // var indented = "";
            // var indentString = new string('\t', indent);
            // foreach (var line in split)
            //     if (!string.IsNullOrEmpty(line))
            //         indented += indentString + line + "\n";
            // children.Add(indented);
            children.Add(s);
        }
        var childrenTable = LuaUtility.CreateTable(state, children);
        try
        {

            state.DoString(Parent.Script);
            var creationF = LuaUtility.GetGlobalF(state, "_Create");
            var returned = CallCreationFunc(state, creationF, Text, childrenTable);
            var result = LuaUtility.GetReturnAs<string>(returned);
            // System.Console.WriteLine($"{Parent.Name} -> {result}");
            // System.Console.WriteLine("AAAAA");
            // System.Console.WriteLine(Parent.Script);
            // System.Console.WriteLine("VVVVV");
            // System.Console.WriteLine();
            return result;

        }
        catch (Exception e)
        {
            throw new Exception($"Failed to create script in {Parent.Name}", e);
        }

    }

    public virtual object[] CallCreationFunc(Lua state, LuaFunction func, string text, LuaTable childrenTable) => func.Call(text, childrenTable);
}