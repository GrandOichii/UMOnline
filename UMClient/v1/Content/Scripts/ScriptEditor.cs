using Godot;
using Godot.Collections;
using System;

public partial class ScriptEditor : Control
{
	[Export]
	public Array<ScriptNode> Effects { get; set; } = [];
	[Export]
	public Array<ScriptNode> Sources { get; set; } = [];
}
