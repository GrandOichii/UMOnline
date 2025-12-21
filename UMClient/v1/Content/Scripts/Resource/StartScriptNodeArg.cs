using Godot;
using System;

[GlobalClass]
public partial class StartScriptNodeArg : Resource
{
    [Export]
    public string Label { get; set; }
    [Export]
    public string BuildMethod { get; set; }
}
