using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ScriptNodeArg : Resource
{
    [Export]
    public string Key { get; set; }
    [Export]
    public string Label { get; set; }
    [Export]
    public ScriptNodeType Accepts { get; set; }
}
