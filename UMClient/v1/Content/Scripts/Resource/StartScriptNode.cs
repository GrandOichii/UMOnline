using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class StartScriptNode : Resource, IScriptNode
{
    [Export]
    public string Name { get; set; }
    [Export]
    public string Title { get; set; }
    [Export]
    public string BuildFunction { get; set; }
    [Export]
    public Array<StartScriptNodeArg> Args { get; set; }
    [Export]
    public PackedScene Scene { get; set; } = 
        ResourceLoader.Load<PackedScene>("res://v1/Content/Scripts/StartScriptNodeNode.tscn"); // ! change if scene location changes

    public string GetDescription() => $"Start node for objects of type '{Title}'";

    public string GetLabel() => Name;

    public (GraphNode, IScriptNodeNode) Instantiate(bool editable)
    {
        var result = Scene.Instantiate() as StartScriptNodeNode;
        result.Value = this;
        result.SetScriptNodeName(Name);
        // * no values to edit, editable is not needed

        return (result, result);
    }

}
