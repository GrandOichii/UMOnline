using Godot;
using System.Collections.Generic;
using System;
using System.Text.Json;

public partial class ScriptNodeNode : GraphNode, IScriptNodeNode
{
    [Export]
    public ScriptNode ScriptNode { get; set; }

    private ScriptEditor _editor;

    public override void _Ready()
    {
        Title = ScriptNode.Label;
        // Modulate = SlotColors[ScriptNode.Type];

        // all nodes need an out slot, so a blank label is always present
        var firstLabel = new Label();
        AddChild(firstLabel);
        SetSlotEnabledRight(0, true);
        SetSlotTypeRight(0, (int)ScriptNode.Type);
        SetSlotColorRight(0, ScriptEditor.GetSlotColor(ScriptNode.Type));

        var lastRightIdx = 0;
        var lastLeftIdx = -1;

        // if is effect, also enable in slot for the previous effect
        if (ScriptNode.Type == ScriptNodeType.Effect)
        {
            ++lastLeftIdx;
            SetSlotEnabledLeft(lastLeftIdx, true);
            SetSlotTypeLeft(lastLeftIdx, (int)ScriptNodeType.Effect);
            SetSlotColorLeft(lastLeftIdx, ScriptEditor.GetSlotColor(ScriptNodeType.Effect));
        }

        // go through all node args 
        foreach (var nodeArg in ScriptNode.NodeArgs)
        {
            ++lastRightIdx;
            ++lastLeftIdx;
            Label label;
            if (lastLeftIdx == 0)
            {
                label = firstLabel;
            }
            else
            {
                label = new Label();
                AddChild(label);
            }
            label.Text = nodeArg.Label;

            SetSlotEnabledLeft(lastLeftIdx, true);
            SetSlotTypeLeft(lastLeftIdx, (int)nodeArg.Accepts);
            SetSlotColorLeft(lastLeftIdx, ScriptEditor.GetSlotColor(nodeArg.Accepts));
        }

        // add simple args
        foreach (var simpleArg in ScriptNode.SimpleArgs)
        {
            AddSimpleArg(simpleArg);
        }
    }

    private void AddSimpleArg(ScriptNodeSimpleArg arg)
    {
        Node child = arg.Type switch
        {
            ScriptNodeSimpleArgType.Checkbox => CreateCheckBoxSimpleArg(arg),
            ScriptNodeSimpleArgType.Option => CreateOptionSimpleArg(arg),
            ScriptNodeSimpleArgType.Number => CreateNumberSimpleArg(arg),
            ScriptNodeSimpleArgType.String => CreateStringSimpleArg(arg),
            _ => throw new Exception($"Unrecognized {nameof(ScriptNodeSimpleArgType)}: {arg.Type}")
        };
        AddChild(child);
    }

    #region Simple arg creation functions

    private LineEdit CreateStringSimpleArg(ScriptNodeSimpleArg arg)
    {
        var result = new LineEdit()
        {
            PlaceholderText = "Enter value"
        };
        result.TextChanged += (_) => _editor.ProcessScriptGraphChange();
        result.Editable = _editable;

        return result;
    }

    private CheckBox CreateCheckBoxSimpleArg(ScriptNodeSimpleArg arg)
    {
        var result = new CheckBox
        {
            Text = arg.Label
        };
        result.Pressed += _editor.ProcessScriptGraphChange;
        result.Disabled = !_editable;

        return result;
    }

    private OptionButton CreateOptionSimpleArg(ScriptNodeSimpleArg arg)
    {
        var result = new OptionButton();

        foreach (var el in arg.Data)
        {
            result.AddItem(el.Label);
            result.SetItemMetadata(result.ItemCount - 1, el.Value);
        }
        result.ItemSelected += (_) => _editor.ProcessScriptGraphChange();
        result.Disabled = !_editable;

        return result;
    }

    private SpinBox CreateNumberSimpleArg(ScriptNodeSimpleArg arg)
    {
        var result = new SpinBox();

        result.ValueChanged += (_) => _editor.ProcessScriptGraphChange();
        result.Editable = _editable;

        return result;
    }

    #endregion

    public bool IsStart() => false;

    public string Generate(
        int forPort,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
    )
    {
        var result = ScriptNode.Script;
        var myInputs = inputs[this];
        var myOutputs = outputs[this];
        int inSlotOffset = ScriptNode.Type == ScriptNodeType.Effect ? 1 : 0;

        // node args
        for (int i = 0; i < ScriptNode.NodeArgs.Count; ++i)
        {
            var input = "!missing connection!";
            if (myInputs.TryGetValue(i + inSlotOffset, out var pair))
            {
                var node = pair.from;
                input = node.Generate(pair.fromPort, inputs, outputs);
            }
            var key = $"${ScriptNode.NodeArgs[i].Key}";
            result = result.Replace(key, input);
        }

        // simple args
        var idx = GetChildCount();
        var simpleArgCount = ScriptNode.SimpleArgs.Count - 1;
        while (simpleArgCount >= 0)
        {
            --idx;
            var arg = ScriptNode.SimpleArgs[simpleArgCount];
            --simpleArgCount;
            var key = $"${arg.Key}";
            var node = GetChildren()[idx];
            var value = GetSimpleValue(arg.Type, node);
            result = result.Replace(key, value);

        }

        if (ScriptNode.Type == ScriptNodeType.Effect && myOutputs.Count == 1)
        {
            var (nextEffect, port) = myOutputs[0];
            var next = nextEffect.Generate(port, inputs, outputs);
            result += $",\n{next}";
        }


        return result;
    }

    private string GetSimpleValue(ScriptNodeSimpleArgType type, Node node)
    {
        return type switch {
            ScriptNodeSimpleArgType.Checkbox => (node as CheckBox).ButtonPressed ? "true" : "false",
            ScriptNodeSimpleArgType.Number => (node as SpinBox).Value.ToString(),
            ScriptNodeSimpleArgType.Option => GetOptionValue(node as OptionButton),
            ScriptNodeSimpleArgType.String => (node as LineEdit).Text,
            _ => throw new Exception($"{nameof(GetSimpleValue)} not implemented for ScriptNodeSimpleArgType {type}")
        };
    }

    private object GetSimpleDataValue(ScriptNodeSimpleArgType type, Node node)
    {
        return type switch {
            ScriptNodeSimpleArgType.Checkbox => (node as CheckBox).ButtonPressed,
            ScriptNodeSimpleArgType.Number => (node as SpinBox).Value,
            ScriptNodeSimpleArgType.Option => (node as OptionButton).Selected,
            ScriptNodeSimpleArgType.String => (node as LineEdit).Text,
            _ => throw new Exception($"{nameof(GetSimpleValue)} not implemented for ScriptNodeSimpleArgType {type}")
        };
    }

    public void SetSimpleValue(ScriptNodeSimpleArgType type, Node node, object value)
    {
        var json = (JsonElement)value;

        switch (type) {
            case ScriptNodeSimpleArgType.Checkbox:
                (node as CheckBox).ButtonPressed = json.GetBoolean();
                break;
            case ScriptNodeSimpleArgType.Number:
                (node as SpinBox).SetValueNoSignal(json.GetDouble());
                break;
            case ScriptNodeSimpleArgType.Option:
                (node as OptionButton).Select(json.GetInt32());
                break;
            case ScriptNodeSimpleArgType.String:
                (node as LineEdit).Text = json.GetString();
                break;
            default:
                throw new Exception($"{nameof(GetSimpleValue)} not implemented for ScriptNodeSimpleArgType {type}");
        };
    }

    public string GetOptionValue(OptionButton node)
    {
        var selected = node.Selected;
        return node.GetItemMetadata(selected).AsString();
    }


    public void SetEssentials(ScriptEditor editor)
    {
        _editor = editor;
    }

    public void LoadState(ScriptNodeState state)
    {
        var idx = GetChildCount();
        var simpleArgCount = ScriptNode.SimpleArgs.Count - 1;
        while (simpleArgCount >= 0)
        {
            --idx;
            var arg = ScriptNode.SimpleArgs[simpleArgCount];
            --simpleArgCount;
            var node = GetChildren()[idx];
            var value = state.Data[arg.Key];            
            SetSimpleValue(arg.Type, node, value);
        }
    }

    private string _scriptNodeName;
    public void SetScriptNodeName(string name)
    {
        _scriptNodeName = name;
    }

    private bool _editable = true;
    public void SetEditable(bool v)
    {
        _editable = v;
    }

    public ScriptNodeState ToState(int id)
    {
        ScriptNodeState result = new()
        {
            Data = [],
            Editor = new() { X = PositionOffset.X, Y = PositionOffset.Y },
            Id = id,
            Name = _scriptNodeName,
        };

        var idx = GetChildCount();
        var simpleArgCount = ScriptNode.SimpleArgs.Count - 1;
        while (simpleArgCount >= 0)
        {
            --idx;
            var arg = ScriptNode.SimpleArgs[simpleArgCount];
            --simpleArgCount;
            var node = GetChildren()[idx];
            var value = GetSimpleDataValue(arg.Type, node);
            result.Data[arg.Key] = value;
        }
        return result;
    }
}
