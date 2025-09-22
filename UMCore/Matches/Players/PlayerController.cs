using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players;

public interface IPlayerController
{
    Task<string> ChooseAction(Player player, string[] options);
    Task<MapNode> ChooseNode(Player player, IEnumerable<MapNode> options, string hint);
    Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint);
    Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint);
    Task<Fighter> ChooseFighter(Player player, IEnumerable<Fighter> options, string hint);
    Task<AvailableAttack> ChooseAttack(Player player, IEnumerable<AvailableAttack> options);
    Task<string> ChooseString(Player player, IEnumerable<string> options, string hint);
}