using Godot;
using System;
using UMCore.Matches;

public partial class MatchConfigEditor : Control
{
    #region Signals

    [Signal]
    public delegate void ChangedEventHandler();

    #endregion

    #region Nodes

    [Export]
    public CheckButton SeedCheckNode { get; set; }
    [Export]
    public SpinBox SeedNode { get; set; }
    [Export]
    public CheckButton RandomFirstPlayerCheckNode { get; set; }
    [Export]
    public SpinBox FirstPlayerIdxNode { get; set; }
    [Export]
    public SpinBox InitialHandSizeNode { get; set; }
    [Export]
    public SpinBox MaxHandSizeNode { get; set; }
    [Export]
    public SpinBox ActionsPerTurnNode { get; set; }
    [Export]
    public SpinBox ManoeuvreDrawAmountNode { get; set; }
    [Export]
    public SpinBox ExhaustDamageNode { get; set; }
    [Export]
    public SpinBox TeamSizeNode { get; set; }

    #endregion

    public void Load(MatchConfig config)
    {
        
    }

    public MatchConfig Build()
    {
        return new MatchConfig()
        {
            ActionsPerTurn = (int)ActionsPerTurnNode.Value,  
            ExhaustDamage = (int)ExhaustDamageNode.Value,
            FirstPlayerIdx = (int)FirstPlayerIdxNode.Value,
            InitialHandSize = (int)InitialHandSizeNode.Value,
            ManoeuvreDrawAmount = (int)ManoeuvreDrawAmountNode.Value,
            MaxHandSize = (int)MaxHandSizeNode.Value,
            RandomFirstPlayer = RandomFirstPlayerCheckNode.ButtonPressed,
            RandomMatch = !SeedCheckNode.ButtonPressed,
            Seed = (int)SeedNode.Value,
            TeamSize = (int)TeamSizeNode.Value
        };
    }

    private void EmitChanged()
    {
        EmitSignalChanged();
    }

    #region Signal connections

    public void OnSeedCheckToggled(bool v)
    {
        SeedNode.Editable = v;
        EmitChanged();
    }

    public void OnRandomFirstPlayerCheckToggled(bool v)
    {
        FirstPlayerIdxNode.Editable = v;
        EmitChanged();
    }

    public void OnSeedValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnFirstPlayerIdxValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnInitialHandSizeValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnMaxHandSizeValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnActionsPerTurnValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnManoeuvreDrawAmountValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnExhaustDamageValueChanged(int v)
    {
        EmitChanged();
    }

    public void OnTeamSizeValueChanged(int v)
    {
        EmitChanged();
    }

    #endregion
}
