using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Effects;
using UMCore.Matches.Players.Cards;
using UMCore.Templates;
using UMCore.Utility;

namespace UMCore.Matches.Players;

public class Player : IHasData<Player.Data>, IHasSetupData<Player.SetupData>
{
    private static readonly List<IAction> ACTIONS = [
        new ManoeuvreAction(),
        new FightAction(),
        new SchemeAction(),
    ];

    private static readonly List<ITurnPhase> TURN_PHASES = [
        new StartPhase(),
        new ActionsPhase(),
        new EndPhase(),
    ];

    private static readonly Dictionary<string, IAction> ACTION_MAP = [];

    static Player()
    {
        foreach (var action in ACTIONS)
            ACTION_MAP.Add(action.Name(), action);
    }

    public Match Match { get; }
    public IPlayerController Controller { get; }
    public string Name { get; }
    public int Idx { get; }
    public int TeamIdx { get; }
    public List<Fighter> Fighters { get; }
    public Hand Hand { get; }
    public Deck Deck { get; }
    public DiscardPile DiscardPile { get; }
    public LoadoutTemplate Loadout { get; }

    public int ActionCount { get; set; }
    // TODO these should be stored in controller
    public List<Log> NewLogs { get; }
    public List<Event> NewEvents { get; }

    public Player(Match match, int idx, string name, int teamIdx, LoadoutTemplate loadout, IPlayerController controller)
    {
        Match = match;
        Controller = controller;
        Name = name;
        Idx = idx;
        Loadout = loadout;

        Hand = new(this);
        Deck = new(this);
        DiscardPile = new(this);

        Fighters = [];
        NewLogs = [];
        NewEvents = [];
    }

    public Log[] PopLogs()
    {
        Log[] result = [.. NewLogs];
        NewLogs.Clear();
        return result;
    }

    public Event[] PopEvents()
    {
        Event[] result = [.. NewEvents];
        NewEvents.Clear();
        return result;
    }

    public async Task InitialPlaceFighters()
    {
        var heroes = Fighters.Where(f => f.IsHero()).ToList();
        foreach (var f in heroes)
            await InitialPlaceFighter(f);

        if (Loadout.StartsWithSidekicks)
        {
            var sidekicks = Fighters.Where(f => f.IsSidekick()).ToList();
            foreach (var f in sidekicks)
                await InitialPlaceFighter(f);
        }


        foreach (var fighter in Fighters)
        {
            await InitialPlaceFighter(fighter);
        }

        Match.Logs.Public($"Player {FormattedLogName} placed all of their fighters");
    }

    public async Task InitialPlaceFighter(Fighter fighter)
    {
        // TODO place in spawn location
        // TODO if spawn location is taken, choose a node clearest to first hero or (if all are surrounded) to sidekick 
        // var spawn = Match.Map.GetSpawnLocation(Idx + (Fighters.Count - 1) * Match.Players.Count);
        // await spawn.PlaceFighter(fighter);

        var nodes = Match.Map.Nodes;
        var node = nodes.First(n => n.Fighter is null);
        await node.PlaceFighter(fighter);

        Match.Logs.Public($"Player {FormattedLogName} placed {fighter.FormattedLogName}");
    }

    public async Task Setup()
    {
        // create and place fighters
        {
            foreach (var template in Loadout.Fighters)
            {
                for (int i = 0; i < template.Amount; ++i)
                {
                    var fighter = new Fighter(this, template);
                    Fighters.Add(fighter);
                }
            }
        }

        // create deck
        {
            foreach (var card in Loadout.Deck)
            {
                await Deck.Add(
                    Enumerable.Range(0, card.Amount)
                        .Select(i => new MatchCard(this, card.Card))
                );
            }
            Deck.Shuffle();
        }

        // initial draw
        var amount = Match.Config.InitialHandSize;
        await Hand.Draw(amount);

        // Match.Logs.Public($"Player {FormattedLogName} drew their initial hand of {amount} cards");
    }

    public IEnumerable<Fighter> GetAliveFighters() => Fighters.Where(f => f.IsAlive());

    public IEnumerable<Fighter> GetAliveHeroes() => GetAliveFighters().Where(f => f.IsHero());

    public string LogName => $"{Name}[{Idx}]";

    public string FormattedLogName => $"{Name}"; // TODO

    public void AddEvent(Event e)
    {
        NewEvents.Add(e);
    }

    public async Task StartTurn()
    {
        Match.Logs.Private(this, "You start your turn", $"Player {FormattedLogName} starts their turn");
        Match.Logger?.LogDebug("Player {LogName} starts their turn", LogName);
        ActionCount = Match.Config.ActionsPerTurn;
    }

    public async Task EndTurn()
    {
        ActionCount = 0;

        while (Hand.Count > Match.Config.MaxHandSize)
        {
            var card = await Controller.ChooseCardInHand(this, Idx, [.. Hand.Cards], $"Discard down to {Match.Config.MaxHandSize} cards");
            await Hand.Discard(card);
        }

        Match.Logs.Private(this, "You end your turn", $"Player {FormattedLogName} ends their turn");

        Match.Logger?.LogDebug("Player {LogName} ends their turn", LogName);
    }

    public bool CanTakeActions()
    {
        // TODO some cards disable taking actions until the end of turn
        return ActionCount > 0 && !Match.IsWinnerDetermined();
    }

    public IEnumerable<IAction> AvailableActions()
    {
        return ACTIONS.Where(a => a.CanBeTaken(this));
    }

    public async Task<IAction> ChooseAction()
    {
        var available = AvailableActions().Select(a => a.Name());
        var chosen = await Controller.ChooseAction(this, [.. available]);

        if (!available.Contains(chosen))
        {
            // TODO add strict mode
        }

        var result = ACTION_MAP[chosen];
        return result;
    }

    public async Task TakeActions()
    {
        Match.Logger?.LogDebug("Starting action phase of player {LogName}", LogName);
        while (CanTakeActions())
        {
            var action = await ChooseAction();
            --ActionCount;
            await action.Execute(this);
        }
    }

    public async Task TakeTurn()
    {
        foreach (var phase in TURN_PHASES)
        {
            await phase.Execute(this);
            if (Match.IsWinnerDetermined()) return;
        }
    }

    public async Task EmitTurnPhaseTrigger(TurnPhaseTrigger trigger)
    {
        Match.Logger?.LogDebug("Emitting turn phase trigger {Trigger} from player {PlayerLogName}", trigger, LogName);

        List<(EffectCollection, Fighter)> effects = [];

        foreach (var fighter in Fighters)
        {
            foreach (var effect in fighter.GetTurnPhaseEffects(trigger))
            {
                effects.Add((effect, fighter));
            }
        }

        // TODO order effects
        // await OrderEffects(in effects);

        foreach (var (effect, fighter) in effects)
        {
            effect.Execute(LuaUtility.CreateTable(Match.LState, new Dictionary<string, object>()
            {
                { "fighter",  fighter },
                { "owner",  this },
            }));
        }

        await Match.UpdateClients();
    }

    public async Task MoveFighters(bool allowBoost = false, bool canMoveOverFriendly = true, bool canMoveOverOpposing = false)
    {
        var boostValue = 0;
        if (allowBoost)
        {
            var card = await ChooseBoostCard();
            if (card is not null)
            {
                await DiscardCardForBoost(card);
                boostValue = card.GetBoostValue();

                Match.Logs.Public($"Player {FormattedLogName} boosts their movement with {card.FormattedLogName}");
            }
        }

        var fighters = GetAliveFighters().ToList();
        while (fighters.Count > 0)
        {
            // var fighter = fighters[0];
            // if (fighters.Count > 1)
            // {
            //     fighter = await Controller.ChooseFighter(this, [..fighters], "Choose which fighter to move");
            // }
            var fighter = await Controller.ChooseFighter(this, [..fighters], "Choose which fighter to move");
            fighters.Remove(fighter);
            await MoveFighter(fighter, fighter.Movement() + boostValue, canMoveOverFriendly, canMoveOverOpposing);
        }
    }

    public async Task MoveFighter(Fighter fighter, int movement, bool canMoveOverFriendly, bool canMoveOverOpposing)
    {
        var available = Match.Map.GetPossibleMovementResults(fighter, movement, canMoveOverFriendly, canMoveOverOpposing);
        var result = await Controller.ChooseNode(this, [..available], $"Choose where to move {fighter.LogName}");
        await result.PlaceFighter(fighter);

        Match.Logs.Public($"Player {FormattedLogName} moves {fighter.FormattedLogName}");
    }

    public async Task<MatchCard?> ChooseBoostCard()
    {
        var card = await Controller.ChooseCardInHandOrNothing(this, Idx, [..Hand.Cards], "Choose a card to boost your movement");
        return card;
    }

    public async Task DiscardCardForBoost(MatchCard card)
    {
        // TODO discard for boost effects
        await Hand.Remove(card);
        await card.PlaceIntoDiscard();
    }

    public async Task Exhaust(int times)
    {
        Match.Logs.Public($"Fighters of player {FormattedLogName} are exhausted {times} times!");

        foreach (var fighter in GetAliveFighters())
        {
            await fighter.ProcessDamage(Match.Config.ExhaustDamage);
        }
    }

    public async Task PlayScheme(MatchCard card, Fighter fighter)
    {
        Match.Events.SchemePlayed(card, fighter);

        await Match.UpdateClients();

        await card.ExecuteSchemeEffects(fighter);

        await Hand.Discard(card);
    }

    public async Task GainActions(int amount)
    {
        ActionCount += amount;

        await Match.UpdateClients();
    }

    public IEnumerable<Fighter> GetFightersThatCanAttack()
    {
        foreach (var fighter in GetAliveFighters())
        {

            yield return fighter;
        }
    }

    public IEnumerable<AvailableAttack> GetAvailableAttacks()
    {
        foreach (var fighter in GetAliveFighters())
        {
            var reachable = fighter.GetReachableFighters();
            if (!reachable.Any()) continue;
            var cards = fighter.GetValidAttackCards();
            if (!cards.Any()) continue;

            foreach (var target in reachable)
            {
                foreach (var card in cards)
                {
                    yield return new()
                    {
                        Fighter = fighter,
                        Target = target,
                        AttackCard = card
                    };
                }
            }

        }
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Idx = Idx,
            Actions = ActionCount,
            Deck = Deck.GetData(player),
            Hand = Hand.GetData(player),
            DiscardPile = DiscardPile.GetData(player),
            Fighters = [.. Fighters.Select(f => f.GetData(player))],
        };
    }

    public SetupData GetSetupData()
    {
        return new()
        {
            DeckName = Loadout.Name,
            Idx = Idx,
            Name = Name,
            Fighters = [.. Fighters.Select(f => f.GetSetupData())],
        };
    }

    public class Data
    {
        public required int Idx { get; init; }
        public required int Actions { get; init; }
        public required MatchCardCollection.Data Deck { get; init; }
        public required MatchCardCollection.Data Hand { get; init; }
        public required MatchCardCollection.Data DiscardPile { get; init; }
        public required Fighter.Data[] Fighters { get; init; }
    }

    public class SetupData
    {
        public required int Idx { get; init; }
        public required string Name { get; init; }
        public required string DeckName { get; init; }
        public required Fighter.SetupData[] Fighters { get; init; }
    }
}
