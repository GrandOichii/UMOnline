using Godot;
using System;

public partial class BotEditor : Control, IPlayerEditor
{
    #region Nodes

    [Export]
    public PlayerEditor PlayerEditorNode { get; set; }

    #endregion

    public PlayerEditorResult Build()
    {
        throw new NotImplementedException();
    }

    public void LoadLocalMatchesTab(LocalMatchesTab lmt)
    {
        PlayerEditorNode.LoadLocalMatchesTab(lmt);
    }

    public void LoadName(string name)
    {
        PlayerEditorNode.LoadName(name);
    }
}
