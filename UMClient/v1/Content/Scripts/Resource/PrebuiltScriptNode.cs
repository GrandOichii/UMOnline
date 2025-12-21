using Godot;
using Godot.Collections;
using System;

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

    public (GraphNode, IScriptNodeNode) Instantiate()
    {
        var result = Scene.Instantiate() as GraphNode;
        return (result, result as IScriptNodeNode);
    }
}