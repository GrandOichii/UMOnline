namespace UMCore.Matches.Players;

public interface IPlayerController
{
    Task<string> ChooseAction(Player player, string[] options);
}