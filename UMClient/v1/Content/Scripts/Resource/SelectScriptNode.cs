using Godot;
using System;

[GlobalClass]
public partial class SelectScriptNode : Resource, IScriptNode
{
    [Export]
    public string Name { get; set; }

    [Export]
    public string Label { get; set; }

    [Export]
    public ScriptNodeType SelectType { get; set; }

    [Export]
    public ScriptNodeType FilterType { get; set; }

    [Export]
    public string SelectMethod { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    [Export]
    public PackedScene Scene { get; set; } = 
        ResourceLoader.Load<PackedScene>("res://v1/Content/Scripts/Prebuilt/Selects/SelectScriptNodeNode.tscn"); // ! change if scene location changes
        
    public string GetDescription() => Description;

    public string GetLabel() => Label;

    public (GraphNode, IScriptNodeNode) Instantiate(bool editable)
    {
        var result = Scene.Instantiate() as SelectScriptNodeNode;
        result.Value = this;
        result.Title = Label;
        result.SetEditable(editable);
        result.SetScriptNodeName(Name);

        return (result, result);
    }

}
