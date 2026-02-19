
using Godot;
using UMCore.Matches.Players;
using UMCore.Templates;

public readonly struct ImageMaps
{
    public required Texture2D CardBack { get; init; }
    public required Godot.Collections.Dictionary<string, Texture2D> Cards { get; init; }
    public required Godot.Collections.Dictionary<string, Texture2D> Fighters { get; init; }
}

public readonly struct PlayerEditorResult
{
    public required string Name { get; init; }
    public required int TeamIdx { get; init; }
    public required LoadoutTemplate Loadout { get; init; }
    public required IPlayerController Controller { get; init; }
    public required ImageMaps? Textures { get; init; }
}

public interface IPlayerEditor
{
    void LoadLocalMatchesTab(LocalMatchesTab lmt);
    PlayerEditorResult Build();
    void LoadName(string name);
    void UpdateDeckLists();
}