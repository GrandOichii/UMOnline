using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Cards;

namespace UMClient.v1.Matches;

// public interface LocalPlayerControllerHandler
// {
//     void ChangeToChooseAction(Player player, string[] options);
// }

public class LocalPlayerController(TestMatch match) : IPlayerController
{
    public Task Update(Player player)
    {
        match.Load(player.Match.GetData(player));
        return Task.CompletedTask;
    }

    #region Choose Action

    public void SetChooseActionResult(string result)
    {
        _chooseActionTask!.SetResult(result);
    }

    private TaskCompletionSource<string> _chooseActionTask = null;
    public Task<string> ChooseAction(Player player, string[] options)
    {
        _chooseActionTask = new();
        // handler.ChangeToChooseAction(player, options);
        // TODO
        return _chooseActionTask.Task;
    }

    #endregion

    public Task<AvailableAttack> ChooseAttack(Player player, IEnumerable<AvailableAttack> options)
    {
        // TODO
        throw new System.NotImplementedException();
    }

    public Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        // TODO
        throw new System.NotImplementedException();
    }

    public Task<MatchCard> ChooseCardInHandOrNothing(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        // TODO
        throw new System.NotImplementedException();
    }

    public Task<Fighter> ChooseFighter(Player player, IEnumerable<Fighter> options, string hint)
    {
        // TODO
        throw new System.NotImplementedException();
    }

    public Task<MapNode> ChooseNode(Player player, IEnumerable<MapNode> options, string hint)
    {
        // TODO
        throw new System.NotImplementedException();
    }

    public Task<string> ChooseString(Player player, IEnumerable<string> options, string hint)
    {
        // TODO
        throw new System.NotImplementedException();
    }
}


public static class DataExtensions {
    public static Variant ToVariant(this Fighter.Data data)
    {
        return new Godot.Collections.Dictionary()
        {
            { "Id", data.Id },
            { "Name", data.Name },
            { "IsAlive", data.IsAlive },
            { "CurHealth", data.CurHealth },
            { "MaxHealth", data.MaxHealth },
        };
    }

    public static Variant ToVariant(this MatchCardCollection.Data data)
    {
        return new Godot.Collections.Dictionary()
        {
            { "Count", data.Count },
            { "Cards", new Godot.Collections.Array(data.Cards.Select(c => Variant.From(c))) },
        };
    }

    public static Variant ToVariant(this Player.Data data)
    {
        return new Godot.Collections.Dictionary()
        {
            { "Idx", data.Idx },
            { "Actions", data.Actions },
            { "Deck", data.Deck.ToVariant() },
            { "Hand", data.Hand.ToVariant() },
            { "DiscardPile", data.DiscardPile.ToVariant() },
            { "Fighters", new Godot.Collections.Array(data.Fighters.Select(p => p.ToVariant())) },
        };
    }

	public static Variant ToVariant(this Match.Data data)
    {
        return new Godot.Collections.Dictionary()
        {
            { "CurPlayerIdx", data.CurPlayerIdx },
            { "Players", new Godot.Collections.Array(data.Players.Select(p => p.ToVariant())) },
        };
    }
}