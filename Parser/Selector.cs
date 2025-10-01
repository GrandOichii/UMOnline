namespace ScriptParser;

public class Selector : ParserBase {

    public Selector() {
        Script = File.ReadAllText("../scripts/selector.lua");
    }

    public override ParseResult Parse(string text) {
        var status = ParseResultStatus.ALL_CHILDREN_FAILED;
        var children = new List<ParseResult>();
        var allDidntMatch = true;

        ParseResult? closestToMatch = null;
        foreach (var child in Children)
        {
            var childResult = child.Parse(text);
            children.Add(childResult);

            if (childResult.Status == ParseResultStatus.DIDNT_MATCH) continue;

            if (closestToMatch is null || childResult.Status == ParseResultStatus.SUCCESS)
                closestToMatch = childResult;
                
            allDidntMatch = false;

            if (childResult.Status == ParseResultStatus.SUCCESS)
            {
                status = ParseResultStatus.SUCCESS;
                break;
            }
        }

        if (status == ParseResultStatus.ALL_CHILDREN_FAILED && allDidntMatch)
            status = ParseResultStatus.DIDNT_MATCH;

        if (status == ParseResultStatus.ALL_CHILDREN_FAILED)
        if (closestToMatch != null)
        {
            foreach (var child in children)
            {
                if (child == closestToMatch!)
                    continue;
                child.Status = ParseResultStatus.IGNORED;
            }
        }
        return new(this, status, text, children);
    }

    public override string ParserType() => "selector";
}