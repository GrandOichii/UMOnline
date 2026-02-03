
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UMCore.Matches.Players;
using UMCore.Templates;
using UMDTO;
using UMModel.Models;

namespace UMModel.Scripts.Scripts;

public class CreateContentUpdate : IScript
{
    public async Task Run(UMContext ctx, string[] args)
    {
        // set active content as inactive
        var cu = await ctx.ContentUpdates.Where(cu => cu.IsActive).FirstOrDefaultAsync();
        if (cu is not null)
        {
            cu.IsActive = false;
        }

        var data = new ContentUpdateGet() {
            Core = (await ctx.CoreScripts.Where(c => c.IsActive).SingleAsync()).Script,
            Loadouts = [.. (await ctx.Loadouts.Where(l => l.IsPublic).Include(l => l.Fighters).Include(l => l.Deck).ToListAsync()).Select(l => l.ToTemplate())]
        };

        var newContentUpdate = new ContentUpdate()
        {
            IsActive = true,
            CreatedDT = DateTime.Now.ToUniversalTime(),
            Description = "", // TODO
            Id = null,
            Data = JsonSerializer.Serialize(data)
        };
        ctx.ContentUpdates.Add(newContentUpdate);
        System.Console.WriteLine($"Created new content update, DT: {newContentUpdate.CreatedDT}");

        await ctx.SaveChangesAsync();
    }
}