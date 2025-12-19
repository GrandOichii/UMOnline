using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ScriptNodeCollection : Resource
{
	[Export]
	public Array<ScriptNode> Effects { get; set; } = [];
	[Export]
	public Array<ScriptNode> Sources { get; set; } = [];
}
