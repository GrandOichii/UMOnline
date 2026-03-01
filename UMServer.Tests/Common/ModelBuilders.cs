using UMModel.Models;

namespace UMServer.Tests.Common;

public class LoadoutBuilder
{
    private readonly Loadout _result = new()
    {
        CantBePlayedWith = [],
        ChoosesSidekick = false,
        Deck = [],
        Fighters = [],
        IsPublic = true,
        MaximumHandSize = 7,
        Name = "Name",
        StartingHandSize = 5,
        StartsWithCards = [],
        StartsWithSidekicks = true
    };

    public LoadoutBuilder IsPublic(bool v)
    {
        _result.IsPublic = v;
        return this;
    }

    public Loadout Build() => _result;
}