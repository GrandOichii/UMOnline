using System.Text.Json;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore;

/// <summary>
/// Player input and output handler
/// </summary>
public interface IIOHandler
{
    /// <summary>
    /// Reads a string
    /// </summary>
    /// <returns>The read string</returns>
    public Task<string> Read();
    public Task Write(UpdateInfo info);
    /// <summary>
    /// Closes the IIOHandler
    /// </summary>
    public Task Close();
}

public class UpdateInfo
{
    public Match.SetupData? Setup { get; init; } = null;
    public required Match.Data Match { get; init; }
    public required int PlayerIdx { get; init; }
    public required string Request { get; init; }
    public required string Hint { get; init; }
    public required Log[] NewLogs { get; init; }
    public required object[] NewEvents { get; init; }
    public required Dictionary<string, object> Args { get; set; }
}

public class IOPlayerController : IPlayerController
{
    /// <summary>
    /// Input and output handler
    /// </summary>
    private readonly IIOHandler _handler;

    public IOPlayerController(IIOHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Write new personalized data
    /// </summary>
    /// <param name="data">New match data</param>
    private async Task WriteData(UpdateInfo data)
    {
        // var json = JsonSerializer.Serialize(data);
        await _handler.Write(data);
    }

    /// <summary>
    /// Turns the provided list to the args value
    /// </summary>
    /// <param name="list">List of values</param>
    /// <returns>Args value</returns>
    private static Dictionary<string, object> ToArgs<T>(T[] options) where T : notnull
    {
        return options.Select(
            (o, i) => new { o, i }
        ).ToDictionary(
            e => e.i.ToString(),
            e => (object)e.o
        );
    }

    public async Task Update(Player player)
    {
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "Update",
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Hint = "",
            Args = [],
        });
    }

    public async Task<string> ChooseAction(Player player, string[] options)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseAction",
            Hint = "Choose action to execute",
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Args = ToArgs(options),
        });

        return await _handler.Read();
        // var idx = int.Parse(await _handler.Read());

        // return options.ToList()[idx];
    }

    public async Task<MapNode> ChooseNode(Player player, MapNode[] options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseNode",
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Hint = hint,
            Args = ToArgs(options.Select(n => n.Id).ToArray()),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        // TODO use playerHandIdx
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseCardInHand",
            NewLogs = player.PopLogs(),
            NewEvents = [.. player.PopEvents().Select<Event, object>(e => e)],
            Hint = hint,
            Args = ToArgs(options.Select(c => c.Id).ToArray()),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, MatchCard[] options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseCardInHandOrNothing",
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Hint = hint,
            Args = ToArgs(options.Select(c => c.Id).ToArray()),
        });

        var read = await _handler.Read();
        if (read.Length == 0) return null;

        return options.ToList()[int.Parse(read)];
    }

    public async Task<Fighter> ChooseFighter(Player player, Fighter[] options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseFighter",
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Hint = hint,
            Args = ToArgs(options.Select(n => n.Id).ToArray()),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, AvailableAttack[] options)
    {
        var attacker = await ChooseFighter(player, [.. options.Select(a => a.Fighter)], "Choose attacker");
        options = [.. options.Where(o => o.Fighter == attacker)];
        var defender = await ChooseFighter(player, [.. options.Select(a => a.Target)], "Choose attacked fighter");
        options = [.. options.Where(o => o.Target == defender)];
        var card = await ChooseCardInHand(player, player.Idx, [.. options.Select(a => a.AttackCard)], "Choose attack card");
        var option = options.First(a => a.AttackCard == card);
        return option;
        
        // TODO replace

        // await WriteData(new()
        // {
        //     PlayerIdx = player.Idx,
        //     Match = player.Match.GetData(player),
        //     Request = "ChooseAttack",
        //     Hint = "Choose attack",
        //     Args = ToArgs(options.Select(n =>
        //     new
        //     {
        //         Card = n.AttackCard.Id,
        //         Fighter = n.Fighter.Id,
        //         Target = n.Target.Id,
        //     })),
        // });

        // var idx = int.Parse(await _handler.Read());

        // return options.ToList()[idx];
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Request = "ChooseString",
            Hint = hint,
            Args = ToArgs(options),
        });

        return await _handler.Read();
    }

    public async Task Setup(Player player, Match.SetupData setupData)
    {
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "Setup",
            Hint = "",
            NewLogs = player.PopLogs(),
            NewEvents = player.PopEvents(),
            Setup = setupData,
            Args = []
        });
    }
}