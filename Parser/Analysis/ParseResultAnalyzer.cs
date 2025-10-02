using System.Data;
using System.Text.Json;
using Parser.Parsers;

namespace Parser;

public class ParseResultAnalyzer
{
    private class AnalyzedText
    {
        public required string Text { get; init; }
        public required string FullText { get; init; }
    }

    private readonly Dictionary<ParserBase, List<AnalyzedText>> UnparsedTexts = [];

    public void AddUnparsed(ParserBase parser, string text, string originalText)
    {
        if (!UnparsedTexts.TryGetValue(parser, out List<AnalyzedText>? value))
        {
            value = [];
            UnparsedTexts.Add(parser, value);
        }

        value.Add(new()
        {
            Text = text,
            FullText = originalText,
        });
    }

    public void Analyze(ParseResult parseResult, string originalText)
    {
        if (parseResult.Status == ParseResultStatus.SUCCESS)
        {
            return;
        }
        if (parseResult.Status == ParseResultStatus.IGNORED)
        {
            return;
        }
        if (parseResult.Status == ParseResultStatus.DIDNT_MATCH)
        {
            AddUnparsed(parseResult.Parent, parseResult.Text, originalText);
            return;
        }

        foreach (var pr in parseResult.Children)
        {
            Analyze(pr, originalText);
        }
    }

    public void Analyze(ParseResult parseResult)
    {
        Analyze(parseResult, parseResult.Text);
    }

    public string ToJson()
    {
        var result = new Dictionary<string, List<AnalyzedText>>();

        foreach (var pair in UnparsedTexts)
        {
            result.Add(pair.Key.Name, pair.Value);
        }

        return JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }
}

