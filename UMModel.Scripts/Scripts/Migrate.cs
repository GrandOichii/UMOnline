
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMModel.Models;

namespace UMModel.Scripts.Scripts;

public class Migrate : IScript
{
    public async Task Run(UMContext ctx, string[] args)
    {
        System.Console.WriteLine("Migrating DB...");
        await ctx.Database.MigrateAsync();
        System.Console.WriteLine("DB migrated");
    }
}