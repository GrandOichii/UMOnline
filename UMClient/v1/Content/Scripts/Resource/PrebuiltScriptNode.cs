using Godot;
using Godot.Collections;
using System;

public interface IPrebuiltNodeNode
{
    void SetEditable(bool v);
}

[GlobalClass]
public partial class PrebuiltScriptNode : Resource, IScriptNode
{
    [Export]
    public string Name { get; set; }
    [Export]
    public string Label { get; set; }
    [Export]
    public ScriptNodeType Type { get; set; }
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }
    [Export]
    public PackedScene Scene { get; set; }

    public string GetDescription() => Description;

    public string GetLabel() => Label;

    public (GraphNode, IScriptNodeNode) Instantiate(bool editable)
    {
        var result = Scene.Instantiate() as GraphNode;
        (result as IPrebuiltNodeNode).SetEditable(editable);
        var node = result as IScriptNodeNode;
        node.SetScriptNodeName(Name);
        return (result, node);
    }
}