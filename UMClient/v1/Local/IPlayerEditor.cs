
using UMCore.Matches.Players;
using UMCore.Templates;

public struct PlayerEditorResult
{
    public required string Name { get; init; }
    public required int TeamIdx { get; init; }
    public required LoadoutTemplate Loadout { get; init; }
    public required IPlayerController Controller { get; init; }
}

public interface IPlayerEditor
{
    void LoadLocalMatchesTab(LocalMatchesTab lmt);
    PlayerEditorResult Build();
    void LoadName(string name);
}