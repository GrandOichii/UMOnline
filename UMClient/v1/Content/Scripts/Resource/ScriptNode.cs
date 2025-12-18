using Godot;
using Godot.Collections;
using System;

public enum ScriptNodeType
{
    Effect = 0,
    Condition = 1,
    Numeric = 2,
    SingleFighter = 3,
    ManyFighters = 4,
    SinglePlayer = 5,
    ManyPlayers = 6,
}

[GlobalClass]
public partial class ScriptNode : Resource
{
    [Export]
    public string Name { get; set; }
    [Export]
    public ScriptNodeType Type { get; set; }
    [Export]
    public Array<ScriptNodeArg> NodeArgs { get; set; } = [];
    [Export]
    public Array<ScriptNodeSimpleArg> SimpleArgs { get; set; } = [];
    [Export(PropertyHint.MultilineText)]
    public string Script { get; set; }
}
