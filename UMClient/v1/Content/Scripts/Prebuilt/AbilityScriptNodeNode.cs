using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AbilityScriptNodeNode : GraphNode, IScriptNodeNode, IPrebuiltNodeNode
{
    [ExportGroup("Nodes")]
    [Export]
    public TextEdit TextEditNode { get; set; }

    public string Generate(
        int forSlot,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
        Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
    )
    {
        var myOutputs = outputs[this];
        if (myOutputs.Count == 0)
        {
            return "!missing connection!";
        }
        var (child, port) = myOutputs[0];
        var childScript = child.Generate(port, inputs, outputs);
        return $"""
        "{TextEditNode.Text}",
        {childScript}
        """;
    }

    public bool IsStart() => false;

    public void LoadState(ScriptNodeState state)
    {
        TextEditNode.Text = state.Data["text"].ToString();
    }

    public void SetEssentials(ScriptEditor editor)
    {
        TextEditNode.TextChanged += editor.ProcessScriptGraphChange;
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
            { "text", TextEditNode.Text }
        },
        Editor = new() { X = PositionOffset.X, Y = PositionOffset.Y },
        Id = id,
        Name = _scriptNodeName,
    };

    public override void _Ready()
    {
        SetSlotEnabledLeft(0, true);
        SetSlotTypeLeft(0, (int)ScriptNodeType.Ability);
        SetSlotColorLeft(0, ScriptEditor.GetSlotColor(ScriptNodeType.Ability));

        SetSlotEnabledRight(0, true);
        SetSlotTypeRight(0, (int)ScriptNodeType.Effect);
        SetSlotColorRight(0, ScriptEditor.GetSlotColor(ScriptNodeType.Effect));
    }

    public void SetEditable(bool v)
    {
        TextEditNode.Editable = v;
    }

}
