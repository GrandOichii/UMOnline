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
	[Export]
	public ConfirmationDialog ToggleScriptTypeDialog { get; set; }

	#endregion

	private Dictionary<string, IScriptNode> _scriptNodeNameMap;

	private IScriptNode GetScriptNodeByName(string name) => _scriptNodeNameMap.GetValueOrDefault(name);

	private Vector2 _lastGraphMousePos;

	private int _scriptId;
	private bool _isManual;
	private bool _editable;

	private string _lastGraphState;

	public void UpdateLastGraphState()
	{
		// nodes
		var id = -1;

		var state = new ScriptState()
		{
			Connections = [],
			Nodes = [],
		};

		Dictionary<string, int> nodeNameToIdMap = [];
		foreach (var child in GraphNode.GetChildren())
		{
			if (child.Name == "_connection_layer") continue;
			var node = child as IScriptNodeNode;
			// var connections = GraphNode.GetConnectionListFromNode(child.Name);

			var scriptState = node.ToState(++id);
			nodeNameToIdMap[child.Name] = id;

			state.Nodes.Add(scriptState);
		}

		// connections
		foreach (var connection in GraphNode.Connections)
		{
			var fromNode = connection["from_node"].AsString();
			var fromPort = connection["from_port"].AsInt32();
			var toNode = connection["to_node"].AsString();
			var toPort = connection["to_port"].AsInt32();

			var fromId = nodeNameToIdMap[fromNode];
			var toId = nodeNameToIdMap[toNode];

			state.Connections.Add(new()
			{
				From = fromId,
				FromSlot = fromPort,
				To = toId,
				ToSlot = toPort
			});
		}

		_lastGraphState = state.ToJson();

		EmitSignal(SignalName.ScriptModelChanged);
	}

	public void LoadScriptModel(ScriptModel script, bool editable)
	{
		_editable = editable;
		_scriptId = script.Id;
		_isManual = script.IsManual;

		ManualEditor.Visible = _isManual;
		GraphEditor.Visible = !_isManual;

		ToggleScriptTypeButtonNode.Disabled = !editable;

		// manual
		ManualScriptEdit.Text = script.Script;
		ManualScriptEdit.Editable = editable;

		// graph
		_lastGraphState = script.GraphState;
		LoadGraphState(script.ParseScriptState());
	}

	private void LoadGraphState(ScriptState state)
	{
		// load other nodes
		Dictionary<int, GraphNode> nodeMap = [];
		foreach (var nodeState in state.Nodes)
		{
			var scriptNode = GetScriptNodeByName(nodeState.Name);

			var (child, node) = scriptNode.Instantiate(_editable);
			node.SetEssentials(this);
			GraphNode.AddChild(child);
			child.SetPositionOffset((new Vector2(nodeState.Editor.X, nodeState.Editor.Y) + GraphNode.ScrollOffset) / GraphNode.Zoom);
			node.LoadState(nodeState);

			nodeMap[nodeState.Id] = child;
		}

		// load connections
		foreach (var connection in state.Connections)
		{
			var from = nodeMap[connection.From];			
			var to = nodeMap[connection.To];

			GraphNode.ConnectNode(
				from.Name,
				connection.FromSlot,
				to.Name,
				connection.ToSlot
			);		
		}

		CallDeferred("RegenerateScript");
	}

	public ScriptModel BuildScriptModel() => new()
	{
		Id = _scriptId,
		Script = _isManual ? ManualScriptEdit.Text : ScriptDisplay.Text,
		IsManual = _isManual,
		GraphState = _lastGraphState,
	};

	public override void _Ready()
	{
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

		foreach (var start in ScriptNodes.GetStarts())
		{
			_scriptNodeNameMap.Add(start.Name, start);
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
			if (categoryMapping.ContainsKey(category))
			{
				foreach (var scriptNode in categoryMapping[category])
				{
					var child = NewNodeTreeListNode.CreateItem(categoryChild);
					child.SetText(0, scriptNode.Label);
					child.SetMetadata(0, scriptNode.Name);
				}
			}
		}
	}

	private void AddScriptNodeToMouseLocation()
	{
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

		var (child, node) = scriptNode.Instantiate(_editable);
		node.SetEssentials(this);
		GraphNode.AddChild(child);
		child.SetPositionOffset((_lastGraphMousePos + GraphNode.ScrollOffset) / GraphNode.Zoom);

		AddNodeWindowNode.Hide();
	}

	private void CancelAdd()
	{
		AddNodeWindowNode.Hide();
	}

	public void ProcessScriptGraphChange()
	{
		RegenerateScript();
		UpdateLastGraphState();
	}

	public void RegenerateScript() {
		// try
		// {
		Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode from, int fromPort)>> inputs = [];
		Dictionary<IScriptNodeNode, Dictionary<int, (IScriptNodeNode to, int toPort)>> outputs = [];
		IScriptNodeNode start = null;

		foreach (var child in GraphNode.GetChildren())
		{
			if (child.Name == "_connection_layer") continue;
			var node = child as IScriptNodeNode;
			if (node.IsStart()) start = node;

			// if (GraphNode.GetConnectionCount(child.Name) == 0) continue;
			var connections = GraphNode.GetConnectionListFromNode(child.Name);
			inputs.Add(node, []);
			outputs.Add(node, []);

			foreach (var connection in connections)
			{
				var toNode = connection["to_node"].AsString();
				var fromNode = connection["from_node"].AsString();
				var toPort = connection["to_port"].AsInt32();
				var fromPort = connection["from_port"].AsInt32();

				if (toNode == child.Name)
				{
					// input
					inputs[node].Add(toPort, (GraphNode.GetNode(fromNode) as IScriptNodeNode, fromPort));
				}
				else
				{
					// output
					outputs[node].Add(fromPort, (GraphNode.GetNode(toNode) as IScriptNodeNode, toPort));
				}
			}
		}

		if (start is null)
		{
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
			return;
		}
		var node = GraphNode.GetNode(nodeName);
		GraphNode.RemoveChild(node);
		node.Free();

		ProcessScriptGraphChange();
	}

	#region Signal connections

	public void OnGraphGuiInput(InputEvent e)
	{
		if (!_editable) return;
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
		if (!_editable) return;
		GraphNode.ConnectNode(fromNode, fromSlot, toNode, toSlot);
		ProcessScriptGraphChange();
	}

	public void OnGraphDisconnectionRequest(string fromNode, int fromSlot, string toNode, int toSlot)
	{
		if (!_editable) return;
		GraphNode.DisconnectNode(fromNode, fromSlot, toNode, toSlot);
		ProcessScriptGraphChange();
	}

	public void OnGraphDeleteNodesRequest(Godot.Collections.Array<string> nodeNames)
	{
		if (!_editable) return;
		foreach (var name in nodeNames)
		{
			TryDelete(name);
		}
	}

	public void OnGraphCopyNodesRequest()
	{
		if (!_editable) return;
		// TODO
		GD.Print("COPY REQUEST");
	}

	public void OnGraphCutNodesRequest()
	{
		if (!_editable) return;
		// TODO
		GD.Print("CUT REQUEST");
	}

	public void OnGraphDuplicateNodesRequest()
	{
		if (!_editable) return;
		// TODO
		GD.Print("DUPLICATE REQUEST");
	}

	public void OnGraphPasteNodesRequest()
	{
		if (!_editable) return;
		// TODO
		GD.Print("PASTE REQUEST");
	}

	public void OnManualScriptEditTextChanged()
	{
		if (!_editable) return;
		EmitSignalScriptModelChanged();
	}

	public void OnGraphEndNodeMove()
	{
		if (!_editable) return;

		UpdateLastGraphState();
	}

	public void OnToggleScriptTypeButtonPressed()
	{
		ToggleScriptTypeDialog.Show();
	}

	public void OnToggleScriptTypeDialogCanceled()
	{
		// ToggleScriptTypeDialog.Hide();
	}

	public void OnToggleScriptTypeDialogConfirmed()
	{
		_isManual = !_isManual;

		ManualEditor.Visible = _isManual;
		GraphEditor.Visible = !_isManual;
		UpdateLastGraphState();
	}

	#endregion
}
