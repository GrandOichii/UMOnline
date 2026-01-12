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

    #endregion

    #region Packed scenes

    [ExportGroup("Packed scenes")]
    [Export]
    public PackedScene TagScene { get; set; }

    #endregion

    public override void _Ready()
    {
        PrefixNode.Text = Prefix;
        NewTagEditNode.PlaceholderText = NewTagPlaceholder;

        while (TagsContainer.GetChildCount() > 0)
            TagsContainer.RemoveChild(TagsContainer.GetChild(0));

        // TODO remove
        Connect(SignalName.TagsChanged, Callable.From((Godot.Collections.Array<string> tags) =>
        {
            GD.Print($"Changed {tags}");
        }));
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

    private List<string> GetTags()
    {
        return [.. TagsContainer.GetChildren().Cast<Tag>().Select(c => c.GetTag())];
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
