using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
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

    [LuaCommand]
    public void DrawCards(int playerIdx, int amount)
    {
        var player = Match.GetPlayer(playerIdx);
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
        return LuaUtility.CreateTable(Match.LState, Match.Fighters.Where(f => f.IsAlive()).ToList());
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
    public void DiscardCard(Player player, int idx)
    {
        var card = player.Hand.Cards[idx];
        player.Hand.Discard(card)
            .Wait();
    }

    [LuaCommand]
    public bool AreOpposingPlayers(Player p1, Player p2)
    {
        // TODO teams
        return p1.Idx != p2.Idx;
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
        fighter.Owner.MoveFighter(fighter, amount, true, canMoveOverOpposing)
            .Wait();
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
        // TODO check for null
        return LuaUtility.CreateTable(Match.LState, node!.GetZones().ToList());

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
        var result = player.Controller.ChooseNode(player, [..options], hint)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public int RecoverHealth(Fighter fighter, int amount)
    {
        var result = fighter.RecoverHealth(amount)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public bool AreAdjacent(Fighter fighter1, Fighter fighter2)
    {
        var node1 = Match.Map.GetFighterLocation(fighter1);
        // TODO check for null
        var node2 = Match.Map.GetFighterLocation(fighter2);

        return node1!.IsAdjecentTo(node2!) || node2!.IsAdjecentTo(node1!);
    }

    [LuaCommand]
    public string ChooseString(Player player, LuaTable optionsRaw, string hint)
    {
        return player.Controller.ChooseString(player, [..LuaUtility.ParseTable<string>(optionsRaw)], hint)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public bool AreInSameZone(Fighter f1, Fighter f2)
    {
        var f2Node = Match.Map.GetFighterLocation(f2);
        // TODO check for null
        return f1.IsInZone(f2Node!.GetZones());
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
    public void CancelCombatEffectsOfOpponent(Player player)
    {
        Match.Combat!.CancelEffectsOfOpponent(player)
            .Wait();
    }

    [LuaCommand]
    public void BoostCardInCombat(Player player, MatchCard card)
    {
        // TODO check combat for null
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
}