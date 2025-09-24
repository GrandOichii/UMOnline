using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using UMCore;
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

public class LocalMatchIOHandler(TestMatch match) : IIOHandler
{
    private TaskCompletionSource<string> _readTask = null;
    public void SetReadTaskResult(string result)
    {
        _readTask.SetResult(result);
    }

    public Task Close()
    {
        // TODO
        return Task.CompletedTask;
    }

    public Task<string> Read()
    {
        _readTask = new();
        // throw new NotImplementedException();

        return _readTask.Task;
    }

    public async Task Write(UpdateInfo info)
    {
        // match.CallDeferred("Load", info.ToVariant());
        match.CallDeferred("Load", Json.ParseString(JsonSerializer.Serialize(info)));
    }
}

// public class LocalPlayerController(TestMatch match) : IOPlayerController
// {
    // #region Choose Action

    // public void SetChooseActionResult(string result)
    // {
    //     _chooseActionTask!.SetResult(result);
    // }

    // private TaskCompletionSource<string> _chooseActionTask = null;
    // public Task<string> ChooseAction(Player player, string[] options)
    // {
    //     _chooseActionTask = new();
    //     // handler.ChangeToChooseAction(player, options);
    //     // TODO
    //     return _chooseActionTask.Task;
    // }

    // #endregion

// }


// public static class DataExtensions
// {
//     public static Variant ToVariant(this UpdateInfo data)
//     {
//         return new Godot.Collections.Dictionary()
//         {
//             { "Match", data.Data.ToVariant() },
//             { "PlayerIdx", data.PlayerIdx },
//             { "Request", data.Request },
//             { "Hint", data.Hint },
//             // { "Args", data.Args }, // TODO
//         };
//     }
//     public static Variant ToVariant(this Fighter.Data data)
//     {
//         return new Godot.Collections.Dictionary()
//         {
//             { "Id", data.Id },
//             { "Name", data.Name },
//             { "IsAlive", data.IsAlive },
//             { "CurHealth", data.CurHealth },
//             { "MaxHealth", data.MaxHealth },
//         };
//     }

//     public static Variant ToVariant(this MatchCardCollection.Data data)
//     {
//         return new Godot.Collections.Dictionary()
//         {
//             { "Count", data.Count },
//             { "Cards", new Godot.Collections.Array(data.Cards.Select(c => Variant.From(c))) },
//         };
//     }

//     public static Variant ToVariant(this Player.Data data)
//     {
//         return new Godot.Collections.Dictionary()
//         {
//             { "Idx", data.Idx },
//             { "Actions", data.Actions },
//             { "Deck", data.Deck.ToVariant() },
//             { "Hand", data.Hand.ToVariant() },
//             { "DiscardPile", data.DiscardPile.ToVariant() },
//             { "Fighters", new Godot.Collections.Array(data.Fighters.Select(p => p.ToVariant())) },
//         };
//     }

//     public static Variant ToVariant(this Match.Data data)
//     {
//         return new Godot.Collections.Dictionary()
//         {
//             { "CurPlayerIdx", data.CurPlayerIdx },
//             { "Players", new Godot.Collections.Array(data.Players.Select(p => p.ToVariant())) },
//         };
//     }
// }