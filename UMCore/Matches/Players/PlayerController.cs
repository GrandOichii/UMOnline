using System.Text.Json;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players;

public interface IPlayerController
{
    Task Setup(Player player);
    Task Update(Player player);
    Task<string> ChooseAction(Player player, string[] options);
    Task<MapNode> ChooseNode(Player player, IEnumerable<MapNode> options, string hint);
    Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint);
    Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint);
    Task<Fighter> ChooseFighter(Player player, IEnumerable<Fighter> options, string hint);
    Task<AvailableAttack> ChooseAttack(Player player, IEnumerable<AvailableAttack> options);
    Task<string> ChooseString(Player player, IEnumerable<string> options, string hint);
}

public class RandomPlayerController(int seed) : IPlayerController
{
    private readonly Random _rnd = new(seed);

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        return options[_rnd.Next(options.Length)];
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, IEnumerable<AvailableAttack> options)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<Fighter> ChooseFighter(Player player, IEnumerable<Fighter> options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<MapNode> ChooseNode(Player player, IEnumerable<MapNode> options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task<string> ChooseString(Player player, IEnumerable<string> options, string hint)
    {
        var opts = options.ToList();
        return opts[_rnd.Next(opts.Count)];
    }

    public async Task Setup(Player player)
    {
    }

    public async Task Update(Player player)
    {
    }
}