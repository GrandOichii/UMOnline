using Godot;
// using Godot.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class ScriptEditor : Control
{
	#region Signals

	[Signal]
	public delegate void ScriptModelChangedEventHandler();

	#endregion

	public static Color GetSlotColor(ScriptNodeType type) => type switch
	{
		ScriptNodeType.Effect => Color.FromHtml("#00ff00"),
		ScriptNodeType.Condition => Color.FromHtml("#ff0000"),
        ScriptNodeType.Ability => Color.FromHtml("#ff8914ff"),
        ScriptNodeType.Numeric => Color.FromHtml("#1450ff"),
        ScriptNodeType.SingleFighter => Color.FromHtml("#00ffff"),
        ScriptNodeType.ManyFighters => Color.FromHtml("#005454"),
        ScriptNodeType.SinglePlayer => Color.FromHtml("#ffff00"),
        ScriptNodeType.ManyPlayers => Color.FromHtml("#565600"),
        ScriptNodeType.FighterFilter => Color.FromHtml("#ff00ff"),
        ScriptNodeType.PlayerFilter => Color.FromHtml("#6b10ff"),
        _ => throw new Exception($"{nameof(GetSlotColor)} not implemented for ScriptNodeType {type}")
	};

	[Export]
	public ScriptNodeCollection ScriptNodes { get; set; }

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public Control GraphEditor { get; set; }
	[Export]
	public Control ManualEditor { get; set; }
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
	[Export]
	public CodeEdit ScriptDisplay { get; set; }
	[Export]
	public CodeEdit ManualScriptEdit { get; set; }
	[Export]
	public Button ToggleScriptTypeButtonNode { get; set; }

	#endregion

	private Dictionary<string, IScriptNode> _scriptNodeNameMap;

	private IScriptNode GetScriptNodeByName(string name) => _scriptNodeNameMap.GetValueOrDefault(name);

	private Vector2 _lastGraphMousePos;

	private int _scriptId;
	private bool _isManual;
	private bool _editable;

	public void LoadScriptModel(ScriptModel script, bool editable)
	{
		_editable = editable;
		_scriptId = script.Id;
		_isManual = script.IsManual;

		ManualEditor.Visible = _isManual;
		GraphEditor.Visible = !_isManual;

		ToggleScriptTypeButtonNode.Disabled = !editable;

		// manual
		ManualScriptEdit.Text = script.ManualScript;
		ManualScriptEdit.Editable = editable;

		// graph
		// TODO
	}

	public ScriptModel BuildScriptModel() => new()
	{
		Id = _scriptId,
		ManualScript = ManualScriptEdit.Text,	
		IsManual = _isManual,
	};

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

		foreach (var prebuilt in ScriptNodes.GetPrebuilts())
		{
			_scriptNodeNameMap.Add(prebuilt.Name, prebuilt);

			var child = NewNodeTreeListNode.CreateItem(root);
			child.SetText(0, prebuilt.Label);
			child.SetMetadata(0, prebuilt.Name);
		}

		var selectChild = NewNodeTreeListNode.CreateItem(root);
		selectChild.SetText(0, "Selects");
		foreach (var select in ScriptNodes.GetSelects())
		{
			_scriptNodeNameMap.Add(select.Name, select);

			var child = NewNodeTreeListNode.CreateItem(selectChild);
			child.SetText(0, select.Label);
			child.SetMetadata(0, select.Name);
		}

		foreach (var category in Enum.GetValues(typeof(ScriptNodeType)).Cast<ScriptNodeType>())
		{
			var categoryChild = NewNodeTreeListNode.CreateItem(root);
			categoryChild.SetText(0, category.ToCategoryName());
			if (categoryMapping.ContainsKey(category)) {
				foreach (var scriptNode in categoryMapping[category])
				{
					var child = NewNodeTreeListNode.CreateItem(categoryChild);
					child.SetText(0, scriptNode.Label);
					child.SetMetadata(0, scriptNode.Name);
				}
			}
			// if (prebuiltCategoryMapping.ContainsKey(category)) {
			// 	foreach (var prebuilt in prebuiltCategoryMapping[category])
			// 	{
			// 		var child = NewNodeTreeListNode.CreateItem(categoryChild);
			// 		child.SetText(0, prebuilt.Label);
			// 		child.SetMetadata(0, prebuilt.Name);
			// 	}
			// }
		}
	}
	
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

		var (child, node) = scriptNode.Instantiate();
		node.SetEssentials(this);
		GraphNode.AddChild(child);
		child.SetPositionOffset((_lastGraphMousePos + GraphNode.ScrollOffset) / GraphNode.Zoom);

		AddNodeWindowNode.Hide();
	}

	private void CancelAdd()
	{
		AddNodeWindowNode.Hide();
	}

	public void RegenerateScript()
	{
		// try
		// {
			Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs = [];
			Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs = [];
			IScriptNodeNode start = null;

			foreach (var child in GraphNode.GetChildren())
			{
				if (child.Name == "_connection_layer") continue;
				var node = child as IScriptNodeNode;
				var connections = GraphNode.GetConnectionListFromNode(child.Name);
				inputs.Add(node, []);
				outputs.Add(node, []);

				foreach (var connection in connections)
				{
					var toNode = connection["to_node"].AsString();
					var fromNode = connection["from_node"].AsString();
					var toPort = connection["to_port"].AsInt32();
					var fromPort = connection["from_port"].AsInt32();

					if (toNode == child.Name) {
						// input
						inputs[node].Add(toPort, (GraphNode.GetNode(fromNode) as IScriptNodeNode, fromPort));
					} else 
					{
						// output
						outputs[node].Add(fromPort, (GraphNode.GetNode(toNode) as IScriptNodeNode, toPort));
					}
				}
				if (node.IsStart()) start = node;
			}

			if (start is null)
			{
				// TODO
				throw new Exception("start not found");
			}

			var script = start.Generate(0, inputs, outputs);
			ScriptDisplay.Text = script;
		// } 
		// catch (Exception e)
		// {
		// 	GD.Print(e);
		// }
	}

	private void TryDelete(string nodeName)
	{
		var connections = GraphNode.GetConnectionListFromNode(nodeName);
		if (connections.Count > 0)
		{
			// TODO notify that cant delete while is connected
			return;
		}
		var node = GraphNode.GetNode(nodeName);
		GraphNode.RemoveChild(node);
		node.Free();
	}

	#region Signal connections

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
		NewScriptNodeNameLabel.Text = scriptNode.GetLabel();
		NewScriptNodeDescriptionLabel.Text = scriptNode.GetDescription();
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

	public void OnGraphConnectionRequest(string fromNode, int fromSlot, string toNode, int toSlot)
	{
		// TODO
		GraphNode.ConnectNode(fromNode, fromSlot, toNode, toSlot);
		RegenerateScript();
	}

	public void OnGraphDisconnectionRequest(string fromNode, int fromSlot, string toNode, int toSlot)
	{
		// TODO
		GraphNode.DisconnectNode(fromNode, fromSlot, toNode, toSlot);
		RegenerateScript();
	}

	public void OnGraphDeleteNodesRequest(Godot.Collections.Array<string> nodeNames)
	{
		foreach (var name in nodeNames)
		{
			TryDelete(name);
		}
	}

	public void OnGraphCopyNodesRequest()
	{
		// TODO
		GD.Print("COPY REQUEST");
	}

	public void OnGraphCutNodesRequest()
	{
		// TODO
		GD.Print("CUT REQUEST");
	}

	public void OnGraphDuplicateNodesRequest()
	{
		// TODO
		GD.Print("DUPLICATE REQUEST");
	}

	public void OnGraphPasteNodesRequest()
	{
		// TODO
		GD.Print("PASTE REQUEST");
	}

	public void OnManualScriptEditTextChanged()
	{
		if (!_editable) return;
		EmitSignalScriptModelChanged();
	}

	#endregion
}
