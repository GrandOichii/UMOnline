using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Templates;
using UMCore.Utility;

namespace UMCore.Matches;

/// <summary>
/// Marks the method as a Lua function
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class LuaCommand : Attribute { }

public class MatchScripts
{
    public Match Match { get; }
    public MatchScripts(Match match)
    {
        Match = match;

        // load all methods into the Lua state
        var type = typeof(MatchScripts);
        foreach (var method in type.GetMethods())
        {
            if (method.GetCustomAttribute(typeof(LuaCommand)) is not null)
            {
                Match.LState[method.Name] = method.CreateDelegate(Expression.GetDelegateType(
                    [.. from parameter in method.GetParameters() select parameter.ParameterType, method.ReturnType]
                ), this);
            }
        }
    }

    public static LuaTable CreateArgs(Fighter fighter, Player owner)
    {
        return LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object?>()
        {
            { "fighter", fighter },
            { "owner", owner },
            { "ctx", LuaUtility.CreateTable(owner.Match.LState) },
        });
    }

    public static LuaTable CreateArgs(Fighter fighter, Player owner, PlacedToken? token)
    {
        return LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object?>()
        {
            { "fighter", fighter },
            { "owner", owner },
            { "token", token },
            { "ctx", LuaUtility.CreateTable(owner.Match.LState) },
        });
    }

    [LuaCommand]
    public void DrawCards(Player player, int amount)
    {
        player.Hand.Draw(amount)
            .Wait();
    }

    [LuaCommand]
    public void GainActions(int playerIdx, int amount)
    {
        var player = Match.GetPlayer(playerIdx);
        player.GainActions(amount)
            .Wait();
    }

    [LuaCommand]
    public LuaTable GetFighters()
    {
        return LuaUtility.CreateTable(Match.LState, Match.Fighters.ToList());
        // return LuaUtility.CreateTable(Match.LState, Match.Fighters.Where(f => f.IsAlive()).ToList());
    }
    
    [LuaCommand]
    public LuaTable GetMovedThroughFighters()
    {
        var movement = Match.CurrentMovement
            ?? throw new MatchException($"Called {nameof(GetMovedThroughFighters)} while there is no movement");

        return LuaUtility.CreateTable(Match.LState, [.. movement.MovedThroughFighters]);
    }

    [LuaCommand]
    public int DealDamage(Fighter fighter, int amount)
    {
        var result = fighter.ProcessDamage(amount)
            .GetAwaiter().GetResult();

        return result;
    }

    [LuaCommand]
    public Combat? GetCombat()
    {
        return Match.Combat;
    }

    [LuaCommand]
    public int Rnd(int max)
    {
        return Match.Random.Next(max);
    }

    [LuaCommand]
    public MatchCard DiscardCard(Player player, int idx)
    {
        var card = player.Hand.Cards[idx];
        player.Hand.Discard(card)
            .Wait();
        return card;
    }

    [LuaCommand]
    public bool AreOpposingPlayers(Player p1, Player p2)
    {
        return p1.IsOpposingTo(p2);
    }

    [LuaCommand]
    public LuaTable GetPlayers()
    {
        return LuaUtility.CreateTable(Match.LState, Match.Players);
    }

    [LuaCommand]
    public int ChooseCardInHand(Player player, Player target, string hint)
    {
        var result = player.Controller.ChooseCardInHand(player, target.Idx, [.. target.Hand.Cards], hint)
            .GetAwaiter().GetResult();
        return target.Hand.GetCardIdx(result);

    }

    [LuaCommand]
    public void MoveFighter(Fighter fighter, int amount, bool canMoveOverOpposing)
    {
        fighter.Owner.MoveFighter(fighter, amount, true, canMoveOverOpposing, false, false)
            .Wait();
    }

    [LuaCommand]
    public void MoveFighters(Player player, LuaTable fightersRaw, int amount, bool canMoveOverOpposing)
    {
        var fighters = LuaUtility.ParseTable<Fighter>(fightersRaw);
        while (fighters.Count > 0)
        {
            var fighter = fighters[0];
            Match.Logger?.LogDebug(fighter.LogName);
            if (fighters.Count > 1)
            {
                fighter = player.Controller.ChooseFighter(player, [.. fighters], "Choose which fighter to move")
                    .GetAwaiter().GetResult();
            }
            fighters.Remove(fighter);

            player.MoveFighter(fighter, amount, true, canMoveOverOpposing, false, false)
                .Wait();
        }
    }

    [LuaCommand]
    public Fighter ChooseFighter(Player player, LuaTable fighters, string hint)
    {
        var options = LuaUtility.ParseTable<Fighter>(fighters);
        var result = player.Controller.ChooseFighter(player, [.. options], hint)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public bool IsOpposingTo(Fighter fighter, Player player)
    {
        return fighter.IsOpposingTo(player);
    }

    [LuaCommand]
    public bool IsCalled(Fighter fighter, string name)
    {
        return fighter.GetName() == name;
    }

    [LuaCommand]
    public int ChooseNumber(Player player, LuaTable optionsRaw, string hint)
    {
        var options = LuaUtility.ParseTable(optionsRaw).Select(o => o.ToString()).ToList();
        var chosen = player.Controller.ChooseString(player, [.. options], hint)
            .GetAwaiter().GetResult();
        return int.Parse(chosen);
    }

    [LuaCommand]
    public LuaTable GetFighterZones(Fighter fighter)
    {
        var node = Match.Map.GetFighterLocation(fighter);
        return LuaUtility.CreateTable(Match.LState, node.GetZones().ToList());

    }

    [LuaCommand]
    public bool IsInZone(MapNode node, LuaTable zonesRaw)
    {
        var zones = LuaUtility.ParseTable(zonesRaw);
        return node.IsInZone(zones);
    }

    [LuaCommand]
    public LuaTable GetNodes()
    {
        return LuaUtility.CreateTable(Match.LState, Match.Map.Nodes);
    }

    [LuaCommand]
    public void PlaceFighter(Fighter fighter, MapNode node)
    {
        node.PlaceFighter(fighter)
            .Wait();
    }

    [LuaCommand]
    public MapNode ChooseNode(Player player, LuaTable nodes, string hint)
    {
        var options = LuaUtility.ParseTable<MapNode>(nodes);
        var result = player.Controller.ChooseNode(player, [.. options], hint)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public int GetHealth(Fighter fighter)
    {
        return fighter.Health.Current;
    }

    [LuaCommand]
    public void SetHealth(Fighter fighter, int value)
    {
        fighter.Health.Current = value;
    }
    
    [LuaCommand]
    public void SetMaxHealth(Fighter fighter, int value)
    {
        fighter.Health.Max = value;
    }

    [LuaCommand]
    public int RecoverHealth(Fighter fighter, int amount)
    {
        var result = fighter.RecoverHealth(amount)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public int FullyRecoverHealth(Fighter fighter)
    {
        var result = fighter.FullyRecoverHealth()
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public bool AreAdjacent(Fighter fighter1, Fighter fighter2)
    {
        var node1 = Match.Map.GetFighterLocation(fighter1);
        var node2 = Match.Map.GetFighterLocation(fighter2);

        return node1.IsAdjecentTo(node2) || node2.IsAdjecentTo(node1);
    }

    [LuaCommand]
    public string ChooseString(Player player, LuaTable optionsRaw, string hint)
    {
        return player.Controller.ChooseString(player, [.. LuaUtility.ParseTable<string>(optionsRaw)], hint)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public bool AreInSameZone(Fighter f1, Fighter f2)
    {
        var f2Node = Match.Map.GetFighterLocation(f2);
        return f1.IsInZone(f2Node.GetZones());
    }

    [LuaCommand]
    public bool AreOpposingInCombat(Fighter f1, Fighter f2)
    {
        var combat = Match.Combat;
        if (combat is null)
        {
            // TODO? throw exception
            return false;
        }
        if (combat.Defender is null)
        {
            return false;
        }
        return (f1 == combat.Attacker && f2 == combat.Defender) ||
                (f2 == combat.Attacker && f1 == combat.Defender);

    }

    [LuaCommand]
    public Player? GetOpponentOf(Player player)
    {
        var combat = Match.Combat
            ?? throw new MatchException($"Called {nameof(GetOpponentOf)} while no combat is active");

        var (part, fighter, _) = combat.GetCombatPart(player);
        var (_, resultFighter) = combat.GetOpponent(part);
        return resultFighter?.Owner;
    }

    [LuaCommand]
    public void CancelCombatEffectsOfOpponent(Player player)
    {
        Match.Combat!.CancelEffectsOfOpponent(player)
            .Wait();
    }

    [LuaCommand]
    public void BoostCardInCombat(Player player, MatchCard card)
    {
        player.Hand.Remove(card)
            .Wait();
        player.Match.Combat!.AddBoostToPlayer(player, card)
            .Wait();
    }

    [LuaCommand]
    public MatchCard GetCardInHand(Player player, int idx)
    {
        return player.Hand.Cards[idx];
    }

    [LuaCommand]
    public bool IsAlive(Fighter fighter)
    {
        return fighter.IsAlive();
    }

    [LuaCommand]
    public Player ChoosePlayer(Player player, LuaTable players, string hint)
    {
        return player.Controller.ChoosePlayer(player, [.. LuaUtility.ParseTable<Player>(players)], hint)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public int GetHandSize(Player player)
    {
        return player.Hand.Count;
    }

    [LuaCommand]
    public bool IsInCombat(Fighter fighter)
    {
        return Match.Combat!.Attacker == fighter || Match.Combat.Defender == fighter;
    }

    [LuaCommand]
    public void DEBUG(object o)
    {
        Match.Logger?.LogDebug(o.ToString());
    }

    [LuaCommand]
    public void DEBUGTABLE(LuaTable table)
    {
        Match.Logger?.LogDebug(table.Keys.Count.ToString());
    }

    [LuaCommand]
    public void LogPublic(string msg)
    {
        Match.Logs.Public(msg);
    }

    [LuaCommand]
    public void SetPlayerStringAttribute(Player player, string key, string value)
    {
        player.Attributes.String.Set(key, value);
    }

    [LuaCommand]
    public string? GetPlayerStringAttribute(Player player, string key)
    {
        return player.Attributes.String.Get(key);
    }

    [LuaCommand]
    public void SetPlayerIntAttribute(Player player, string key, int value)
    {
        player.Attributes.Int.Set(key, value);
    }

    [LuaCommand]
    public int? GetPlayerIntAttribute(Player player, string key)
    {
        return player.Attributes.Int.Get(key);
    }

    [LuaCommand]
    public LuaTable GetCardsInDiscardPile(Player player)
    {
        return LuaUtility.CreateTable(Match.LState, player.DiscardPile.Cards);
    }

    [LuaCommand]
    public LuaTable GetDeck(Player player)
    {
        return LuaUtility.CreateTable(Match.LState, player.Deck.Cards);
    }

    [LuaCommand]
    public bool CardHasLabel(MatchCard card, string label)
    {
        return card.HasLabel(label);
    }

    [LuaCommand]
    public bool IsMoving(Fighter fighter)
    {
        if (Match.CurrentMovement is null) return false;
        return Match.CurrentMovement.Fighter == fighter;
    }

    [LuaCommand]
    public Movement? GetcurrentMovement()
    {
        return Match.CurrentMovement;
    }

    [LuaCommand]
    public LuaTable GetTokens()
    {
        List<PlacedToken> tokens = [.. Match.Map.Nodes.SelectMany(n => n.Tokens)];
        return LuaUtility.CreateTable(Match.LState, tokens);
    }

    [LuaCommand]
    public PlacedToken ChooseToken(Player player, LuaTable tokens, string hint)
    {
        var options = LuaUtility.ParseTable<PlacedToken>(tokens);
        var result = player.Controller.ChooseToken(player, [.. options], hint)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public bool IsUnoccupied(MapNode node)
    {
        return !node.IsOccupied();
    }

    [LuaCommand]
    public bool IsTokenDefined(string tokenName)
    {
        return Match.Tokens.IsDefined(tokenName);
    }

    [LuaCommand]
    public void PlaceToken(MapNode node, string tokenName)
    {
        var token = Match.Tokens.Get(tokenName);
        node.PlaceToken(token)
            .Wait();
    }

    [LuaCommand]
    public int GetTokenAmount(string tokenName)
    {
        return Match.Tokens.Get(tokenName).Amount;
    }

    [LuaCommand]
    public void CancelCurrentMovement()
    {
        Match.CurrentMovement!.Cancel();
    }

    [LuaCommand]
    public void RemoveToken(PlacedToken token)
    {
        token.Remove()
            .Wait();
    }

    [LuaCommand]
    public async Task RemoveFighterFromBoard(Fighter fighter)
    {
        Match.Map.RemoveFighterFromBoard(fighter)
            .Wait();

    }

    [LuaCommand]
    public Fighter GetDefender()
    {
        return Match.Combat!.Defender;
    }
    
    [LuaCommand]
    public Fighter GetAttacker()
    {
        return Match.Combat!.Attacker;
    }

    [LuaCommand]
    public bool NodeContainsToken(MapNode node, string tokenName)
    {
        return node.HasToken(tokenName);
    }

    [LuaCommand]
    public bool FighterStandsOn(Fighter fighter, MapNode node)
    {
        return node.Fighter == fighter;
    }

    [LuaCommand]
    public bool IsCardOfCharacter(MatchCard card, string name)
    {
        return card.Template.IsCardOfCharacter(name);
    }

    [LuaCommand]
    public void SetFighterName(Fighter fighter, string name)
    {
        fighter.SetName(name);
    }

    [LuaCommand]
    public MapNode? GetFighterNode(Fighter fighter)
    {
        return Match.Map.GetFighterLocationOrDefault(fighter);
    }

    [LuaCommand]
    public bool AreNodesAdjacent(MapNode node1, MapNode node2)
    {
        return node1.IsAdjecentTo(node2);
    }

    [LuaCommand]
    public void MoveToken(PlacedToken token, MapNode node)
    {
        token.MoveTo(node)
            .Wait();
    }

    [LuaCommand]
    public bool IsDefeated(Fighter fighter)
    {
        return !fighter.IsAlive();
    }

    [LuaCommand]
    public bool IsNodeEmpty(MapNode node)
    {
        return node.IsEmpty();
    }

    [LuaCommand]
    public LuaTable Mill(Player player, int amount)
    {
        var result = player.Mill(amount)
            .GetAwaiter().GetResult();

        return LuaUtility.CreateTable(Match.LState, result);
    }

    [LuaCommand]
    public int? GetBoostValue(MatchCard card)
    {
        return card.GetBoostValue();
    }

    [LuaCommand]
    public void AddToCardValueInCombat(Player player, int amount)
    {
        var combat = Match.Combat
            ?? throw new MatchException($"Called {nameof(AddToCardValueInCombat)} while there is no combat");

        var (card, _, _) = combat.GetCombatPart(player);
        if (card is null) return;

        card.Value += amount;
    }

    [LuaCommand]
    public LuaTable GetCombatPart(Player player)
    {
        var combat = Match.Combat
            ?? throw new MatchException($"Called {nameof(GetCombatPart)} while there is no combat");
        var (part, fighter, isDefense) = combat.GetCombatPart(player);
        return LuaUtility.CreateTable(Match.LState, new List<object?>() { part, fighter, isDefense });
    }

    [LuaCommand]
    public void PutCardOnTheBottomOfDeck(Player player, MatchCard? card)
    {
        if (card is null)
            throw new MatchException($"Provided null card for {nameof(PutCardOnTheBottomOfDeck)}");
        try
        {
            var deck = player.Deck;
            deck.PutOnBottom(card)
                .Wait();
        }catch (Exception e)
        {
            Match.Logger?.LogError(e.Message);
            Match.Logger?.LogError(e.StackTrace);
            Match.Logger?.LogError(e.InnerException?.Message);
            Match.Logger?.LogError(e.InnerException?.StackTrace);
        }
    }
    
    [LuaCommand]
    public LuaTable RemoveCombatPart(Player player)
    {
        var combat = Match.Combat
            ?? throw new MatchException($"Called {nameof(RemoveCombatPart)} while there is no combat");

        var (part, fighter) = combat.RemoveCombatPart(player);
        return LuaUtility.CreateTable(Match.LState, new List<object?>() { part, fighter});
    }

    [LuaCommand]
    public Fighter GetMovingFighter()
    {
        var movement = Match.CurrentMovement
            ?? throw new MatchException($"Called {nameof(GetMovingFighter)} while there is no movement");

        return movement.Fighter;
    }

    [LuaCommand]
    public bool PerformedActionThisTurn(Player player, string action)
    {
        return player.TurnHistory.PerformedAction(action);
    }

    [LuaCommand]
    public bool FighterAttackedThisTurn(Fighter fighter)
    {
        return fighter.Owner.TurnHistory.AttackedWithFighter(fighter);
    }

    [LuaCommand]
    public bool IsWinnerDetermined()
    {
        return Match.IsWinnerDetermined();
    }

    [LuaCommand]
    public Player GetCurrentPlayer()
    {
        return Match.CurrentPlayer();
    }

    [LuaCommand]
    public int GetLostCounter(Player player)
    {
        return player.TurnHistory.GetLostCounter();
    }

    [LuaCommand]
    public void AddAtTheStartOfNextTurnEffect(Player player, Fighter effectSource, LuaTable effects)
    {
        player.AtTheStartOfTurnTemporaryEffects.Add((effectSource, new(effects)));
    }

    [LuaCommand]
    public void ShuffleDiscardIntoDeck(Player player)
    {
        var cards = player.DiscardPile.TakeFromTop(player.DiscardPile.Count)
            .GetAwaiter().GetResult();

        player.Deck.Add(cards)
            .Wait();

        player.Deck.Shuffle();
    }

    [LuaCommand]
    public void AddAtTheEndOfMovementEffect(Fighter fighter, LuaTable effectCollection)
    {
        var movement = Match.CurrentMovement
            ?? throw new MatchException($"Called {nameof(AddAtTheEndOfMovementEffect)} while there is no movement");

        movement.AtTheEndOfMovementEffects.Add((fighter, new(effectCollection)));
    }
}