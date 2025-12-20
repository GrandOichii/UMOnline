using Godot;
// using Godot.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class ScriptEditor : Control
{
	[Export]
	public PackedScene ScriptNodeNodeScene { get; set; }

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
	[Export]
	public Container NewNodeInfoContainerNode { get; set; }
	[Export]
	public Label NewScriptNodeNameLabel { get; set; }
	[Export]
	public RichTextLabel NewScriptNodeDescriptionLabel { get; set; }

	#endregion

	private Dictionary<string, ScriptNode> _scriptNodeNameMap;
	private ScriptNode GetScriptNodeByName(string name) => _scriptNodeNameMap[name];

	public override void _Ready() {
		AddNodeWindowNode.Hide();
		NewNodeInfoContainerNode.Hide();
		ResetAddNewNodeWindow();

		// create tree
		var root = NewNodeTreeListNode.CreateItem();
		NewNodeTreeListNode.HideRoot = true;

		var categoryMapping = new Dictionary<ScriptNodeType, List<ScriptNode>>();
		_scriptNodeNameMap = [];
		foreach (var scriptNode in ScriptNodes.GetScriptNodes())
		{
			_scriptNodeNameMap.Add(scriptNode.Name, scriptNode);
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
					child.SetText(0, scriptNode.Label);
					child.SetMetadata(0, scriptNode.Name);
				}
			}
		}
		// var child2 = NewNodeTreeListNode.CreateItem(root);
		// var subchild1 = NewNodeTreeListNode.CreateItem(child1);
		// subchild1.set_text(0, "Subchild1");
	}
	
	private Vector2 _lastGraphMousePos;
	private void AddScriptNodeToMouseLocation() {
		_lastGraphMousePos = GraphNode.GetLocalMousePosition();
		ResetAddNewNodeWindow();
		AddNodeWindowNode.Show();
	}

	private void ResetAddNewNodeWindow()
	{
		NewNodeInfoContainerNode.Hide();
	}

	private void AddNewSelectedNode()
	{
		var selected = NewNodeTreeListNode.GetSelected();
		var scriptNodeName = selected.GetMetadata(0).AsString();
		if (scriptNodeName.Length == 0) return;
		var scriptNode = GetScriptNodeByName(scriptNodeName);

		var child = ScriptNodeNodeScene.Instantiate() as ScriptNodeNode;
		GraphNode.AddChild(child);
		child.SetPositionOffset((_lastGraphMousePos + GraphNode.ScrollOffset) / GraphNode.Zoom);
		child.Load(scriptNode);

		AddNodeWindowNode.Hide();
	}

	private void CancelAdd()
	{
		AddNodeWindowNode.Hide();
	}

	#region Signals

	public void OnGraphGuiInput(InputEvent e) 
	{
		if (e.IsActionPressed("add_script_node"))
		{
			AddScriptNodeToMouseLocation();
		}
	}

	public void OnNewNodeTreeListItemSelected()
	{
		var selected = NewNodeTreeListNode.GetSelected();
		var scriptNodeName = selected.GetMetadata(0).AsString();
		if (scriptNodeName.Length == 0) return;

		NewNodeInfoContainerNode.Show();
		var scriptNode = GetScriptNodeByName(scriptNodeName);
		NewScriptNodeNameLabel.Text = scriptNode.Label;
		NewScriptNodeDescriptionLabel.Text = scriptNode.Description;
	}

	public void OnNewNodeTreeListItemActivated()
	{
		AddNewSelectedNode();
	}

	public void OnAddNewNodeButtonPressed()
	{
		AddNewSelectedNode();
	}

	public void OnCancelAddNewNodeButtonPressed()
	{
		CancelAdd();
	}

	public void OnAddNodeWindowCloseRequested()
	{
		CancelAdd();
	}

	#endregion
}
