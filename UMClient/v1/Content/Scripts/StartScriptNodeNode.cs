using Godot;
using System;
using System.Collections.Generic;

public partial class StartScriptNodeNode : GraphNode, IScriptNodeNode
{
	[Export]
	public StartScriptNode Value { get; set; }

	public override void _Ready()
	{
		Title = Value.Title;

		int lastIdx = -1;
		foreach (var arg in Value.Args)
		{
			++lastIdx;
			var child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Text = arg.Label
			};
			AddChild(child);
			SetSlotEnabledRight(lastIdx, true);
			SetSlotTypeRight(lastIdx, (int)ScriptNodeType.Ability);
			SetSlotColorRight(lastIdx, ScriptEditor.GetSlotColor(ScriptNodeType.Ability));
		}
	}

	public bool IsStart() => true;

	public string Generate(
        int forSlot,
		Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs,
		Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs
	)
	{
		var myOutputs = outputs[this];
		List<string> childScripts = [];

		for (int i = 0; i < Value.Args.Count; ++i)
		{
			if (!myOutputs.ContainsKey(i)) continue;
			var (toNode, toSlot) = myOutputs[i];
			var arg = Value.Args[i];
			var script = toNode.Generate(toSlot, inputs, outputs);
			childScripts.Add($"""
			:{arg.BuildMethod}(
			{script}
			)
			""");
		}

		var inner = string.Join("\n", childScripts);
		return $"""
		function _Create()
		return {Value.BuildFunction}()
		{inner}
		:Build()
		end
		""";
	}

    public void SetEssentials(ScriptEditor editor)
    {
    }
}
