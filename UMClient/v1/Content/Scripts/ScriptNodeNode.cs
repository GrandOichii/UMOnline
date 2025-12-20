using Godot;
using Godot.Collections;
using System;

public partial class ScriptNodeNode : GraphNode
{
    [Export]
    public Dictionary<ScriptNodeType, Color> SlotColors { get; set; }

    public void Load(ScriptNode scriptNode)
    {
        Title = scriptNode.Label;
        // Modulate = SlotColors[scriptNode.Type];

        // all nodes need an out slot, so a blank label is always present
        var firstLabel = new Label();
        AddChild(firstLabel);
        SetSlotEnabledRight(0, true);
        SetSlotTypeRight(0, (int)scriptNode.Type);
        SetSlotColorRight(0, SlotColors[scriptNode.Type]);

        var lastRightIdx = 0;
        var lastLeftIdx = -1;

        // if is effect, also enable in slot for the previous effect
        if (scriptNode.Type == ScriptNodeType.Effect)
        {
            ++lastLeftIdx;
            SetSlotEnabledLeft(lastLeftIdx, true);
            SetSlotTypeLeft(lastLeftIdx, (int)ScriptNodeType.Effect);
            SetSlotColorLeft(lastLeftIdx, SlotColors[ScriptNodeType.Effect]);
            // TODO add colors
            // SetSlotColorLeft()
        }

        // go through all node args 
        foreach (var nodeArg in scriptNode.NodeArgs)
        {
            ++lastRightIdx;
            ++lastLeftIdx;
            Label label;
            if (lastLeftIdx == 0)
            {
                label = firstLabel;
            } else {
                label = new Label();
                AddChild(label);
            }
            label.Text = nodeArg.Label;

            SetSlotEnabledLeft(lastLeftIdx, true);
            SetSlotTypeLeft(lastLeftIdx, (int)nodeArg.Accepts);
            SetSlotColorLeft(lastLeftIdx, SlotColors[nodeArg.Accepts]);
        }

        // add simple args
        foreach (var simpleArg in scriptNode.SimpleArgs)
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
        };
        AddChild(child);
    }

    private CheckBox CreateCheckBoxSimpleArg(ScriptNodeSimpleArg arg)
    {
        var result = new CheckBox();

        result.Text = arg.Label;

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

        return result;
    }

    private SpinBox CreateNumberSimpleArg(ScriptNodeSimpleArg arg)
    {
        var result = new SpinBox();

        return result;
    }
}
