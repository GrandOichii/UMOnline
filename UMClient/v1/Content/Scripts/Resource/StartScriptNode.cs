using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class StartScriptNode : Resource
{
    [Export]
    public string Title { get; set; }
    [Export]
    public string BuildFunction { get; set; }
    [Export]
    public Array<StartScriptNodeArg> Args { get; set; }
}
