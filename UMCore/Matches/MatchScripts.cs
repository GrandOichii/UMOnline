using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualBasic;
using NLua;
using UMCore.Matches.Attacks;
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
    public void DealDamage(Fighter fighter, int amount)
    {
        fighter.ProcessDamage(amount)
            .Wait();
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
        System.Console.WriteLine($"DISCARD {player.LogName} CARD {idx}");

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
        var result = player.Controller.ChooseCardInHand(player, target.Idx, target.Hand.Cards, hint)
            .GetAwaiter().GetResult();
        return target.Hand.GetCardIdx(result);

    }

    [LuaCommand]
    public void MoveFighter(Fighter fighter, int amount)
    {
        fighter.Owner.MoveFighter(fighter, amount, true, false)
            .Wait();
    }

    [LuaCommand]
    public Fighter ChooseFighter(Player player, LuaTable fighters, string hint)
    {
        var options = LuaUtility.ParseTable<Fighter>(fighters);
        var result = player.Controller.ChooseFighter(player, options, hint)
            .GetAwaiter().GetResult();
        return result;
    }

    [LuaCommand]
    public bool IsOpposingTo(Fighter fighter, Player player)
    {
        return fighter.IsOpposingTo(player);
    }
}