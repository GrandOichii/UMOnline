using Godot;
using System;
using UMCore.Matches.Players;

public partial class BotEditor : Control, IPlayerEditor
{
    #region Nodes

    [Export]
    public PlayerEditor PlayerEditorNode { get; set; }

    #endregion

    public PlayerEditorResult Build()
    {
        var result = PlayerEditorNode.Build();
        
        // TODO choose player controller based on option picked
        return new()
        {
            Controller = new RandomPlayerController(0),
            Loadout = result.Loadout,
            Name = result.Name,
            TeamIdx = result.TeamIdx,
            Textures = result.Textures,
        };
    }
    
    public void UpdateDeckLists()
    {
        PlayerEditorNode.UpdateDeckLists();
    }

    public void LoadLocalMatchesTab(LocalMatchesTab lmt)
    {
        PlayerEditorNode.LoadLocalMatchesTab(lmt);
    }

    public void LoadName(string name)
    {
        PlayerEditorNode.LoadName(name);
    }

    #region Signal connections

    public void OnRemoveButtonPressed()
    {
        QueueFree();
    }

    #endregion
}
