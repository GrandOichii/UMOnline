using Godot;
using System;

public partial class Tag : PanelContainer
{
    #region Nodes

    [Export]
    public Label NameNode { get; set; }

    #endregion

    private TagsEditor _parent;

    public string GetTag() => NameNode.Text;

    public void SetTagsEditor(TagsEditor parent)
    {
        _parent = parent;
    }

    public void Load(string tag)
    {
        NameNode.Text = tag;
    }

    #region Signal connections

    public void OnRemoveButtonPressed()
    {
        QueueFree();
        _parent.EmitWithout(GetTag());
    }

    #endregion
}
