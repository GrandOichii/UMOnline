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
    private static Dictionary<string, object> ToArgs<T>(IEnumerable<T> options) where T : notnull
    {
        return options.Select(
            (o, i) => new { o, i }
        ).ToDictionary(
            e => e.i.ToString(),
            e => (object)e.o // TODO? this looks bad
        );
    }

    public async Task Update(Player player)
    {
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "Update",
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
            Args = ToArgs(options),
        });

        return await _handler.Read();
        // var idx = int.Parse(await _handler.Read());

        // return options.ToList()[idx];
    }

    public async Task<MapNode> ChooseNode(Player player, IEnumerable<MapNode> options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseNode",
            Hint = hint,
            Args = ToArgs(options.Select(n => n.Id)),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        // TODO use playerHandIdx
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseCardInHand",
            Hint = hint,
            Args = ToArgs(options.Select(c => c.Id)),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseCardInHandOrNothing",
            Hint = hint,
            Args = ToArgs(options.Select(c => c.Id)),
        });

        var read = await _handler.Read();
        if (read.Length == 0) return null;

        return options.ToList()[int.Parse(read)];
    }

    public async Task<Fighter> ChooseFighter(Player player, IEnumerable<Fighter> options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseFighter",
            Hint = hint,
            Args = ToArgs(options.Select(n => n.Id)),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, IEnumerable<AvailableAttack> options)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseAttack",
            Hint = "Choose attack",
            Args = ToArgs(options.Select(n =>
            new
            {
                Card = n.AttackCard.Id,
                Fighter = n.Fighter.Id,
                Target = n.Target.Id,
            })),
        });

        var idx = int.Parse(await _handler.Read());

        return options.ToList()[idx];
    }

    public async Task<string> ChooseString(Player player, IEnumerable<string> options, string hint)
    {
        await WriteData(new() {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "ChooseString",
            Hint = hint,
            Args = ToArgs(options),
        });

        return await _handler.Read();
    }

    public async Task Setup(Player player)
    {
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "Setup",
            Hint = "",
            Args = []
        });
    }

    public async Task Setup(Player player, Match.SetupData setupData)
    {
        await WriteData(new()
        {
            PlayerIdx = player.Idx,
            Match = player.Match.GetData(player),
            Request = "Setup",
            Hint = "",
            Setup = setupData,
            Args = []
        });
    }
}