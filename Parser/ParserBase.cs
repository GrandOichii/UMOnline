using NLua;

namespace ScriptParser;

public abstract class ParserBase {
    public required string Name { get; set; }
    public List<ParserBase> Children { get; set; } = [];
    public string Script { get; set; } = "function _Create() return 'error(\\'script not specified\\')' end";

    public abstract string ParserType();
    abstract public ParseResult Parse(string text);
}