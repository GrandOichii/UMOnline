using UMCore.Templates;

namespace UMCore.Matches;

public class QueuedPlayerCollection(MatchConfig config)
{
    private List<Player> _players = [];

    public void AddPlayer(string name, int teamIdx, LoadoutTemplate loadout)
    {
        _players.Add(new()
        {
            Name = name,
            Loadout = loadout,
            TeamIdx = teamIdx
        });
    }

    public bool CanRun()
    {
        var teams = new List<Player>[config.TeamCount];
        for (int i = 0; i < config.TeamCount; ++i)
        {
            teams[i] = [];
        }
        foreach (var player in _players)
        {
            if (player.TeamIdx >= config.TeamCount) return false;
            teams[player.TeamIdx].Add(player);
        }

        // team checks
        var teamCount = teams[0].Count;
        foreach (var team in teams)
        {
            if (team.Count > config.TeamSize)
            {
                return false;
            }
            if (teamCount != team.Count) return false;
        }

        return true;
    }

    public class Player
    {
        public required string Name { get; init; }
        public required int TeamIdx { get; set; }
        public required LoadoutTemplate Loadout { get; set; }
    }
}