using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TagsEditor : VBoxContainer
{
    #region Signals

    [Signal]
    public delegate void TagsChangedEventHandler(Godot.Collections.Array<string> tags);

    #endregion

    #region Exports 

    [Export]
    public string Prefix { get; set; }
    [Export]
    public string NewTagPlaceholder { get; set; }

    #endregion

    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public Label PrefixNode { get; set; }
    [Export]
    public LineEdit NewTagEditNode { get; set; }
    [Export]
    public Container TagsContainer { get; set; }
    [Export]
    public Button AddTagButton { get; set; }

    #endregion

    #region Packed scenes

    [ExportGroup("Packed scenes")]
    [Export]
    public PackedScene TagScene { get; set; }

    #endregion

    private bool _editable = true;

    public bool IsEditable() => _editable;

    public override void _Ready()
    {
        PrefixNode.Text = Prefix;
        NewTagEditNode.PlaceholderText = NewTagPlaceholder;

        RemoveTags();
    }

    private void RemoveTags()
    {
        while (TagsContainer.GetChildCount() > 0)
            TagsContainer.RemoveChild(TagsContainer.GetChild(0));
    }

    public void LoadTags(string[] tags)
    {
        RemoveTags();

        foreach (var tag in tags)
            AddTag(tag);
    }

    private void AddTag(string newTag)
    {
        if (newTag.Length == 0) return;

        NewTagEditNode.Clear();

        var child = TagScene.Instantiate<Tag>();
        child.SetTagsEditor(this);
        TagsContainer.AddChild(child);

        child.Load(newTag);
    }

    public List<string> GetTags()
    {
        return [.. TagsContainer.GetChildren().Cast<Tag>().Where(c => !c.IsQueuedForDeletion()).Select(c => c.GetTag())];
    }

    private void EmitTagsChanged(List<string> tags)
    {
        EmitSignal(
            SignalName.TagsChanged,
            new Godot.Collections.Array(
                tags.Select(t => Variant.From(t))
            )
        );
    }

    public void EmitWithout(string tag)
    {
        var tags = GetTags();
        tags.Remove(tag);

        EmitTagsChanged(tags);
    }

    public void SetEditable(bool value)
    {
        _editable = value;
        AddTagButton.Disabled = !value;
        NewTagEditNode.Editable = value;
    }

    #region Signal connections

    public void OnAddTagButtonPressed()
    {
        // TODO
        var newTag = NewTagEditNode.Text;
        AddTag(newTag);

        EmitTagsChanged(GetTags());
    }

    public void OnNewTagEditTextSubmitted(string newText)
    {
        AddTag(newText);
    }

    #endregion
}
