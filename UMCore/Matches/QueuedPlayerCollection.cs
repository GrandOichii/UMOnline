using UMCore.Templates;

namespace UMCore.Matches;

public class QueuedPlayerCollection(MatchConfig config)
{
    public List<Player> Players { get; } = [];

    public void AddPlayer(string name, int teamIdx, LoadoutTemplate loadout)
    {
        Players.Add(new()
        {
            Name = name,
            Loadout = loadout,
            TeamIdx = teamIdx
        });
    }

    public string CanRun()
    {
        if (Players.Count == 0)
        {
            return "No players";
        }

        var teams = new List<Player>[config.TeamCount];
        for (int i = 0; i < config.TeamCount; ++i)
        {
            teams[i] = [];
        }
        foreach (var player in Players)
        {
            if (Players.Count(p => p.Name == player.Name) > 1)
            {
                return $"Duplicate name: {player.Name}";
            }
            if (Players.Count(p => p.Loadout.Name == player.Loadout.Name) > 1)
                return $"Duplicate decks: {player.Loadout.Name}";
            if (Players.Any(p =>
                p.Loadout.CantBePlayedWith.Contains(player.Loadout.Name) ||
                player.Loadout.CantBePlayedWith.Contains(p.Loadout.Name)
            )) return $"Two players have decks that can't be played with each other";

            if (player.TeamIdx >= config.TeamCount)
                return $"Team {player.TeamIdx} has a TeamIdx that is not allowed ({player.TeamIdx}, max team count: {config.TeamCount})";

            teams[player.TeamIdx].Add(player);
        }

        // team checks
        var teamCount = teams[0].Count;
        for (int i = 0; i < teams.Length; ++i)
        {
            var team = teams[i];
            if (team.Count > config.TeamSize)
            {
                return $"Team {i} has too many players";
            }
            if (teamCount != team.Count)
                return $"Teams are not balanced, team {i} has {team.Count}, players, team 0 has {teamCount}";
        }

        return string.Empty;
    }

    public class Player
    {
        public required string Name { get; init; }
        public required int TeamIdx { get; set; }
        public required LoadoutTemplate Loadout { get; set; }
    }
}