
using Microsoft.EntityFrameworkCore;

namespace UMModel.Scripts.Scripts;

public class UpdateCoreScript : IScript
{
    public async Task Run(UMContext ctx, string[] args)
    {
        if (args.Length == 1)
        {
            throw new Exception($"Core script path not specified for {nameof(UpdateCoreScript)}");
        }
        var coreScriptPath = args[1];

        System.Console.WriteLine($"Updating core script from path {coreScriptPath}");

        var script = File.ReadAllText(coreScriptPath);

        System.Console.WriteLine("Read script, checking if need to apply any changes");

        var active = await ctx.CoreScripts.SingleAsync(s => s.IsActive);
        if (active.Script == script)
        {
            System.Console.WriteLine("Specified script is identical to the active script, cancelling changes");
            return;
        }

        System.Console.WriteLine("Changes detected, marking all scripts as inactive");
        
        foreach (var entry in ctx.CoreScripts)
            entry.IsActive = false;

        System.Console.WriteLine("Adding new core script");

        ctx.CoreScripts.Add(new()
        {
            CreatedAt = DateTime.Now.ToUniversalTime(),
            IsActive = true,
            Script = script,
        });

        await ctx.SaveChangesAsync();

        System.Console.WriteLine("Updated core script");
    }
}