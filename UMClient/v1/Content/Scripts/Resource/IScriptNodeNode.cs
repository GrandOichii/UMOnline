using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IScriptNodeNode
{
    public bool IsStart();
    public string Generate(
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
    );

    public void SetEssentials(
        ScriptEditor editor
    );

    public static string RepeatString(string text, int n)
    {
        return new StringBuilder(text.Length * n)
            .Insert(0, text, n)
            .ToString();
    }
}