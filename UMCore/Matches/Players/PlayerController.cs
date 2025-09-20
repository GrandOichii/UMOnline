namespace UMCore.Matches.Players;

public interface IPlayerController
{
    Task<string> ChooseAction(Player player, string[] options);
    Task<MapNode> PromptNode(Player player, IEnumerable<MapNode> options, string hint);
}