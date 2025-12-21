using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ScriptNodeCollection : Resource
{
	[Export]
	public Array<ScriptNode> Basic { get; set; } = [];
	[Export]
	public Array<PrebuiltScriptNode> Prebuilts { get; set; }
	[Export]
	public Array<SelectScriptNode> Selects { get; set;}

	public List<ScriptNode> GetScriptNodes()
	{
		return [.. Basic];
	}

	public List<PrebuiltScriptNode> GetPrebuilts()
	{
		return [.. Prebuilts];
	}

	public List<SelectScriptNode> GetSelects()
	{
		return [.. Selects];
	}
}
