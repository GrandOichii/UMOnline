using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class SelectScriptNodeNode : GraphNode, IScriptNodeNode
{
    [Export]
    public SelectScriptNode Value { get; set; }

    [ExportGroup("Nodes")]
    [Export]
    public Button AddFilterButton { get; set; }
    [Export]
    public CheckBox SingleCheckNode { get; set; }

    public override void _Ready()
    {
        // out
        SetSlotEnabledRight(0, true);
        SetSlotTypeRight(0, (int)Value.SelectType);
        SetSlotColorRight(0, ScriptEditor.GetSlotColor(Value.SelectType));
    }

    #region Signals connections

    public void OnAddFilterButtonPressed()
    {
        var child = new Button();
        child.Text = "Remove";
        int childIdx = GetChildCount();
        child.Pressed += () =>
        {
            GD.Print($"REMOVE CHILD {childIdx}");  
            // TODO
        };

        AddChild(child);
        SetSlotEnabledLeft(childIdx, true);
        SetSlotTypeLeft(childIdx, (int)Value.FilterType);
        SetSlotColorLeft(childIdx, ScriptEditor.GetSlotColor(Value.FilterType));
    }

    #endregion

    public bool IsStart() => false;

    public string Generate(
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
    )
    {
        var myInputs = inputs[this];
        var slotOffset = 2;
        var slotCount = GetChildCount() - slotOffset;

        List<string> filters = [];
        for (int i = 0; i < slotCount; ++i)
        {
            var filter = "!missing connection!";
            if (myInputs.TryGetValue(i, out var pair))
            {
                filter = $":{pair.from.Generate(inputs, outputs)}";
            }
            filters.Add(filter);
        }

        var inner = string.Join("\n", filters);

        return $"""
        {Value.SelectMethod}()
        {inner}
        :Build()
        """;
    }

    public void SetEssentials(ScriptEditor editor)
    {
        // TODO
    }
}
