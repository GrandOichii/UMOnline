using Godot;
using Godot.Collections;
using System;

public enum ScriptNodeSimpleArgType
{
    Checkbox = 0,
    Option = 1,
}


[GlobalClass]
public partial class ScriptNodeSimpleArg : Resource
{
    [Export]
    public string Key { get; set; }
    [Export]
    public string Label { get; set; }
    [Export]
    public ScriptNodeSimpleArgType Type { get; set; }
    [Export]
    public Array<ScriptNodeSimpleArgDataElement> Data { get; set; }
}
