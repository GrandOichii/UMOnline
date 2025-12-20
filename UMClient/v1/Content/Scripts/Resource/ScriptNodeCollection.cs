using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ScriptNodeCollection : Resource
{
	[Export]
	public Array<ScriptNode> Effects { get; set; } = [];
	[Export]
	public Array<ScriptNode> Sources { get; set; } = [];

	public List<ScriptNode> GetScriptNodes()
	{
		return [.. Effects, .. Sources];
	}
}
