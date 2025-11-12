
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMModel.Models;

namespace UMModel.Scripts.Scripts;

public class ImportLoadouts : IScript
{
    public async Task Run(UMContext ctx, string[] args)
    {
        if (args.Length == 1)
        {
            throw new Exception($"Loadout path not specified for {nameof(ImportLoadouts)}");
        }
        var loadoutsPath = args[1];

        System.Console.WriteLine($"Importing loadouts from path {loadoutsPath}");

        // remove old entries
        await ctx.Database.ExecuteSqlAsync($"DELETE FROM \"Cards\";");
        await ctx.Database.ExecuteSqlAsync($"DELETE FROM \"Fighters\";");
        await ctx.Database.ExecuteSqlAsync($"DELETE FROM \"Loadouts\";");

        foreach (var dir in Directory.GetDirectories(loadoutsPath))
        {
            var dirName = Path.GetFileName(dir);
            System.Console.WriteLine($"Importing loadout {dirName}");
            var loadoutPath = Path.Join(dir, $"{dirName}.json");
            var template = JsonSerializer.Deserialize<LoadoutTemplate>(File.ReadAllText(loadoutPath))
                ?? throw new Exception($"Failed to parse loadout data from path {loadoutPath}");

            foreach (var card in template.Deck)
            {
                card.Script = File.ReadAllText(Path.Join(dir, card.Script));
            }

            foreach (var fighter in template.Fighters)
            {
                fighter.Script = File.ReadAllText(Path.Join(dir, fighter.Script));
            }
            var loadout = Loadout.FromTemplate(template);
            ctx.Loadouts.Add(loadout);
        }

        await ctx.SaveChangesAsync();
    }
}