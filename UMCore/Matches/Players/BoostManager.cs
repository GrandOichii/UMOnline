using System.Threading.Tasks;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players.Cards;

namespace UMCore.Matches.Players;

[Flags]
public enum BoostTarget
{
    MOVEMENT = 1,
    COMBAT = 2,
    ALL = MOVEMENT | COMBAT
}

public class BoostSource(MatchCardCollection collection, BoostTarget allowedTargets)
{
    public MatchCardCollection Collection { get; } = collection;
    public BoostTarget AllowedTargets { get; } = allowedTargets;

    public BoostSource(MatchCardCollection collection) : this(collection, BoostTarget.ALL) { }

    public async Task<MatchCard?> ChooseBoostCard(Player player)
    {
        if (Collection.Cards.Count == 0) throw new MatchException($"Something went wrong in method GetBoostSource of class {nameof(BoostManager)}: provided empty card zone for {nameof(ChooseBoostCard)}");

        var choice = await player.Controller.ChooseCardOrNothing(player, [.. Collection.Cards.Where(c => c.CanBeUsedAsBoost()) ], "Choose a card to BOOST with");
        return choice;
    }
}

public class BoostManager(Player player)
{
    public Player Player = player;
    public Match Match = player.Match;

    public Dictionary<string, BoostSource> Sources { get; } = new() {
        { player.Hand.GetName(), new(player.Hand) },
    };

    public void AddBoostSource(MatchCardCollection collection, BoostTarget allowedTargets)
    {
        Sources.Add(collection.GetName(), new(collection, allowedTargets));
    }

    private async Task<BoostSource?> GetBoostSource(BoostTarget target)
    {
        List<BoostSource> sources = [.. Sources.Values.Where(s => (s.AllowedTargets & target) == target && s.Collection.Cards.Count > 0)];

        if (sources.Count == 0) return null;

        var result = sources[0];

        if (sources.Count > 1)
        {
            var choice = await Player.Controller.ChooseString(Player, [.. sources.Select(s => s.Collection.GetName())], "Choose BOOST source");
            result = Sources[choice];
        }

        return result;
    }

    public async Task<(int, bool)> TryBoostMovement()
    {
        var source = await GetBoostSource(BoostTarget.COMBAT);
        if (source is null) return (0, false);

        var card = await source.ChooseBoostCard(Player);
        if (card is null) return (0, false);

        card.Move(source.Collection, card.Owner.DiscardPile, ZoneChangeLocation.BOTTOM, ZoneChangeType.DISCARDED);

        var result = (int)card.GetBoostValue()!;

        Match.Logs.Public($"Player {Player.FormattedLogName} boosts their movement with {card.FormattedLogName}");

        ExecuteOnBoostEffects();
        return (result, true);
    }

    public async Task<MatchCard?> TryBoostCombat(CombatPart combatPart)
    {
        var source = await GetBoostSource(BoostTarget.COMBAT);
        if (source is null) return null;

        var card = await source.ChooseBoostCard(Player);
        if (card is null) return null;

        await combatPart.AddBoost(source.Collection, card);
        
        ExecuteOnBoostEffects();

        return card;
    }

    private void ExecuteOnBoostEffects()
    {
        var effects = Match.GetEffectCollectionThatAccepts(new(Player), f => f.OnBoostEffects);
        // TODO order effects
        foreach (var (source, effect) in effects)
            effect.Execute(new(source), new(Player));

    }
}