using Godot;
using System;
using System.Collections.Generic;

public partial class IfScriptNodeNode : GraphNode, IScriptNodeNode
{
    public override void _Ready()
    {
        // effect
        SetSlotEnabledLeft(0, true);
        SetSlotTypeLeft(0, (int)ScriptNodeType.Effect);
        SetSlotColorLeft(0, ScriptEditor.GetSlotColor(ScriptNodeType.Effect));
        SetSlotEnabledRight(0, true);
        SetSlotTypeRight(0, (int)ScriptNodeType.Effect);
        SetSlotColorRight(0, ScriptEditor.GetSlotColor(ScriptNodeType.Effect));

        // condition
        SetSlotEnabledLeft(1, true);
        SetSlotTypeLeft(1, (int)ScriptNodeType.Condition);
        SetSlotColorLeft(1, ScriptEditor.GetSlotColor(ScriptNodeType.Condition));

        // true
        SetSlotEnabledRight(1, true);
        SetSlotTypeRight(1, (int)ScriptNodeType.Effect);
        SetSlotColorRight(1, ScriptEditor.GetSlotColor(ScriptNodeType.Effect));
        // false
        SetSlotEnabledRight(2, true);
        SetSlotTypeRight(2, (int)ScriptNodeType.Effect);
        SetSlotColorRight(2, ScriptEditor.GetSlotColor(ScriptNodeType.Effect));
    }

    public bool IsStart() => false;

    public string Generate(
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
    )
    {
        return "TODO";
    }

    public void SetEssentials(ScriptEditor editor)
    {
        // TODO
    }

}
