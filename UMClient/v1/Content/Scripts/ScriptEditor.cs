using Godot;
// using Godot.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class ScriptEditor : Control
{
	[Export]
	public ScriptNodeCollection ScriptNodes { get; set; }

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public GraphEdit GraphNode { get; set; }
	[Export]
	public Window AddNodeWindowNode { get; set; }
	[Export]
	public Tree NewNodeTreeListNode { get; set; }

	#endregion

	public override void _Ready() {
		AddNodeWindowNode.Hide();

		// create tree
		var root = NewNodeTreeListNode.CreateItem();
		NewNodeTreeListNode.HideRoot = true;

		var categoryMapping = new Dictionary<ScriptNodeType, List<ScriptNode>>();

		// var scriptNodeMapping = new Dictionary<string, ScriptNode>();
		foreach (var scriptNode in ScriptNodes.Effects)
		{
			if (!categoryMapping.ContainsKey(scriptNode.Type))
			{
				categoryMapping.Add(scriptNode.Type, []);
			}
			categoryMapping[scriptNode.Type].Add(scriptNode);
		}

		foreach (var category in Enum.GetValues(typeof(ScriptNodeType)).Cast<ScriptNodeType>())
		{
			var categoryChild = NewNodeTreeListNode.CreateItem(root);
			categoryChild.SetText(0, category.ToLabel());
			if (categoryMapping.ContainsKey(category)) {
				foreach (var scriptNode in categoryMapping[category])
				{
					var child = NewNodeTreeListNode.CreateItem(categoryChild);
					GD.Print($"Label: {scriptNode.Label}");
					child.SetText(0, scriptNode.Label);
				}
			}
		}
		// var child2 = NewNodeTreeListNode.CreateItem(root);
		// var subchild1 = NewNodeTreeListNode.CreateItem(child1);
		// subchild1.set_text(0, "Subchild1");
	}
	
	private void AddScriptNodeToMouseLocation() {
		AddNodeWindowNode.Show();
	}

	#region Signals

	public void OnGraphGuiInput(InputEvent e) 
	{
		if (e.IsActionPressed("add_script_node"))
		{
			AddScriptNodeToMouseLocation();
		}
	}

	#endregion
}
