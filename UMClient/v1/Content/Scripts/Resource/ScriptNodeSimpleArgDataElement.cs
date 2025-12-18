using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ScriptNodeSimpleArgDataElement : Resource
{
    [Export]
    public string Label { get; set; }
    [Export]
    public string Value { get; set; }
}