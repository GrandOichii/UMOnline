using Godot;
using Godot.Collections;
using System;

public enum ScriptNodeType : int
{
    Effect = 0,
    Ability = 9,
    Condition = 1,
    Numeric = 2,
    SingleFighter = 3,
    ManyFighters = 4,
    SinglePlayer = 5,
    ManyPlayers = 6,
    FighterFilter = 7,
    PlayerFilter = 8,
}

public static class ScriptNodeTypeExtensions
{
    public static string ToCategoryName(this ScriptNodeType t) => t switch 
    {
        ScriptNodeType.Effect => "Effects",
        ScriptNodeType.Ability => "Abilities",
        ScriptNodeType.Condition => "Conditions",
        ScriptNodeType.Numeric => "Numerics",
        ScriptNodeType.SingleFighter => "Single fighters",
        ScriptNodeType.ManyFighters => "Multiple fighters",
        ScriptNodeType.SinglePlayer => "Single players",
        ScriptNodeType.ManyPlayers => "Many players",
        ScriptNodeType.FighterFilter => "Fighter filters",
        ScriptNodeType.PlayerFilter => "Player filters",
        _ => throw new Exception($"{nameof(ToCategoryName)} not implemented for ScriptNodeType {t}")
    };

    public static string ToLabel(this ScriptNodeType t) => t switch 
    {
        ScriptNodeType.Effect => "Effect",
        ScriptNodeType.Ability => "Abilitiy",
        ScriptNodeType.Condition => "Conditions",
        ScriptNodeType.Numeric => "Numeric",
        ScriptNodeType.SingleFighter => "Single fighter",
        ScriptNodeType.ManyFighters => "Multiple fighters",
        ScriptNodeType.SinglePlayer => "Single player",
        ScriptNodeType.ManyPlayers => "Many players",
        ScriptNodeType.FighterFilter => "Fighter filter",
        ScriptNodeType.PlayerFilter => "Player filter",
        _ => throw new Exception($"{nameof(ToCategoryName)} not implemented for ScriptNodeType {t}")
    };
}

[GlobalClass]
public partial class ScriptNode : Resource, IScriptNode
{
    [Export]
    public string Name { get; set; }
    [Export]
    public string Label { get; set; }
    [Export]
    public ScriptNodeType Type { get; set; }
    [Export]
    public Array<ScriptNodeArg> NodeArgs { get; set; } = [];
    [Export]
    public Array<ScriptNodeSimpleArg> SimpleArgs { get; set; } = [];
    [Export(PropertyHint.MultilineText)]
    public string Script { get; set; }
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }
    [Export]
    public PackedScene ScriptNodeNodeScene { get; set; } = 
        ResourceLoader.Load<PackedScene>("res://v1/Content/Scripts/ScriptNodeNode.tscn"); // ! change if scene location changes

    public string GetDescription() => Description;

    public string GetLabel() => Label;

    public (GraphNode, IScriptNodeNode) Instantiate()
    {
        var result = ScriptNodeNodeScene.Instantiate() as ScriptNodeNode;
        result.ScriptNode = this;

        return (result, result);
    }

}
