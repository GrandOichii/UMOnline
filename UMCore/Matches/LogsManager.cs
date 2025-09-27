using UMCore.Matches.Players;

namespace UMCore.Matches;

public class LogsManager(Match match)
{
    public void Public(string msg)
    {
        foreach (var p in match.Players)
        {
            p.AddLog(new()
            {
                Message = msg,
                IsPrivate = false,
            });
        }
    }

    public void Private(Player player, string privateMsg, string publicMsg)
    {
        foreach (var p in match.Players)
        {
            if (p == player)
            {
                p.AddLog(new()
                {
                    Message = privateMsg,
                    IsPrivate = true,
                });
                continue;
            }
            p.AddLog(new()
            {
                Message = publicMsg,
                IsPrivate = false,
            });
        }
    }
}

public class Log
{
    public required string Message { get; init; }
    public required bool IsPrivate { get; init; }
}
