using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

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

        SetSlotEnabledRight(1, true);
        SetSlotTypeRight(1, (int)ScriptNodeType.Numeric);
        SetSlotColorRight(1, ScriptEditor.GetSlotColor(ScriptNodeType.Numeric));
    }

    public void AddFilter()
    {
        var child = new Button
        {
            Text = "Remove"
        };
        int childIdx = GetChildCount();
        child.Pressed += () =>
        {
            if (!_editable) return;
            
            GD.Print($"REMOVE CHILD {childIdx}");  
            // TODO
        };

        AddChild(child);
        SetSlotEnabledLeft(childIdx, true);
        SetSlotTypeLeft(childIdx, (int)Value.FilterType);
        SetSlotColorLeft(childIdx, ScriptEditor.GetSlotColor(Value.FilterType));
    }

    #region Signals connections

    public void OnAddFilterButtonPressed()
    {
        AddFilter();
    }

    #endregion

    public bool IsStart() => false;

    public int GetSlotCount() => GetChildCount() - 2;

    public string Generate(
        int forSlot,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
    )
    {
        var myInputs = inputs[this];

        var slotCount = GetSlotCount();

        List<string> filters = [];
        for (int i = 0; i < slotCount; ++i)
        {
            var filter = "!missing connection!";
            if (myInputs.TryGetValue(i, out var pair))
            {
                filter = $":{pair.from.Generate(pair.fromPort, inputs, outputs)}";
            }
            filters.Add(filter);
        }

        var inner = string.Join("\n", filters);

        var result = $"""
        {Value.SelectMethod}()
        {inner}
        :Build()
        """;

        if (forSlot == 1)
        {
            result = $"""
            UM.Number:Count(
            {result}
            )
            """;
        }

        return result;
    }

    public void SetEssentials(ScriptEditor editor)
    {
    }

    public void LoadState(ScriptNodeState state)
    {
        SingleCheckNode.ButtonPressed = ((JsonElement)state.Data["single"]).GetBoolean();

        var outputCount = ((JsonElement)state.Data["outputCount"]).GetInt32();
        for (int i = 0; i < outputCount; ++i)
            AddFilter();
    }

    private string _scriptNodeName;
    public void SetScriptNodeName(string name)
    {
        _scriptNodeName = name;
    }

    public ScriptNodeState ToState(int id) => new()
    {
        Data = new()
        {
            { "single", SingleCheckNode.ButtonPressed },
            { "outputCount", GetSlotCount() },
        },
        Editor = new() { X = PositionOffset.X, Y = PositionOffset.Y },
        Id = id,
        Name = _scriptNodeName,
    };

    private bool _editable;
    public void SetEditable(bool v)
    {
        _editable = v;
        AddFilterButton.Disabled = !v;
        SingleCheckNode.Disabled = !v;
    }
}
