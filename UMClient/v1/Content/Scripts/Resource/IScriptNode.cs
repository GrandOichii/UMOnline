

using Godot;

public interface IScriptNode
{
    public string GetLabel();
    public string GetDescription();
    public (GraphNode, IScriptNodeNode) Instantiate();
}