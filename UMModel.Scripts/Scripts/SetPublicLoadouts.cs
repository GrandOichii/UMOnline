
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMModel.Models;

namespace UMModel.Scripts.Scripts;

public class SetPublicLoadouts : IScript
{
    public async Task Run(UMContext ctx, string[] args)
    {
        var loadouts = await ctx.Loadouts.ToListAsync();
        var publicLoadoutNames = args[1.. ];

        foreach (var loadout in loadouts)
        {
            loadout.IsPublic = publicLoadoutNames.Contains(loadout.Name);
            Console.WriteLine($"Setting loadout {loadout.Name} IsPublic state to: {loadout.IsPublic}");
            ctx.Loadouts.Update(loadout);
        }

        await ctx.SaveChangesAsync();
    }
}