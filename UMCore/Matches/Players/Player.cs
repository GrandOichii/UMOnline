using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;
using UMCore.Matches.Players.Cards;
using UMCore.Templates;

namespace UMCore.Matches.Players;

public class Player
{
    private static readonly List<IAction> ACTIONS = [
        new ManoeuvreAction(),
        new FightAction(),
        new SchemeAction(),
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

    public int ActionCount { get; set; }

    public Player(Match match, int idx, string name, int teamidx, IPlayerController controller)
    {
        Match = match;
        Controller = controller;
        Name = name;
        Idx = idx;

        Hand = new(this);
        Deck = new(this);
        DiscardPile = new(this);

        Fighters = [];
    }

    public async Task Setup()
    {
        // create and place fighters
        {
            // TODO remove
            var fighter = new Fighter(this)
            {
                Hero = true,
                Name = $"f_{LogName}",
                Position = Match.Map.Template.GetSpawnNode(Idx)
            };
            Fighters.Add(fighter);
            // Fighters.Add(new(this)
            // {
            //     Hero = false,
            // });

            // TODO prompt player to place fighters
            var spawn = Match.Map.GetSpawnLocation(Idx);
            await spawn.PlaceFighter(fighter);
        }

        // create deck
        {
            // TODO remove
            CardTemplate template1 = new()
            {
                Name = "Test Card 1",
                Type = "Scheme",
                Value = 0,
                Boost = 1,
                Text = "Test Card Text",
                Script = "test script",
            };
            CardTemplate template2 = new()
            {
                Name = "Test Card 2",
                Type = "Versatile",
                Value = 0,
                Boost = 3,
                Text = "Test Card Text",
                Script = "test script",
            };

            var template1Count = 5;
            var template2Count = 5;
            await Deck.Add(
                Enumerable.Range(0, template1Count)
                    .Select(i => new MatchCard(this, template1))
            );
            await Deck.Add(
                Enumerable.Range(0, template2Count)
                    .Select(i => new MatchCard(this, template2))
            );
        }
    }    

    public string LogName => $"{Name}[{Idx}]";

    public async Task StartTurn()
    {
        Match.Logger?.LogDebug("Player {LogName} starts their turn", LogName);
        ActionCount = 2; // TODO move to configuration
    }

    public async Task EndTurn()
    {
        Match.Logger?.LogDebug("Player {LogName} ends their turn", LogName);
        ActionCount = 0;
    }

    public bool CanTakeActions()
    {
        // TODO some cards disable taking actions until the end of turn
        return ActionCount > 0;
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
            // TODO
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
            await action.Execute(this);
            --ActionCount;
        }
    }

    public async Task TakeTurn()
    {
        await StartTurn();

        await TakeActions();

        await EndTurn();
    }

    public async Task MoveFighters(bool allowBoost = false, bool canMoveOverFriendly = true, bool canMoveOverOpposing = false)
    {
        var boostValue = 0;
        // if (allowBoost)
        // {
        //     var card = await ChooseBoostCard();
        //     if (card is not null)
        //     {
        //         await DiscardCardForBoost(card);
        //         boostValue = card.GetBoostValue();
        //     }
        // }
        // TODO allow player to choose order
        foreach (var fighter in Fighters)
        {
            await MoveFighter(fighter, fighter.Movement() + boostValue, canMoveOverFriendly, canMoveOverOpposing);
        }

        // TODO
    }

    public async Task MoveFighter(Fighter fighter, int movement, bool canMoveOverFriendly, bool canMoveOverOpposing)
    {
        var available = Match.Map.GetPossibleMovementResults(fighter, movement, canMoveOverFriendly, canMoveOverOpposing);
        var result = await Controller.PromptNode(this, available, $"Choose where to move {fighter.LogName}");
        await result.PlaceFighter(fighter);
    }

    public async Task<MatchCard?> ChooseBoostCard()
    {
        // TODO
        throw new NotImplementedException();
    }

    public async Task DiscardCardForBoost(MatchCard card)
    {
        // TODO? any discard for boost effects (if they exist)
        // TODO
        throw new NotImplementedException();
    }
}