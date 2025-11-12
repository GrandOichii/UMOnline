
using Microsoft.EntityFrameworkCore;

namespace UMModel.Scripts.Scripts;

public class TestScript : IScript
{
    public async Task Run(UMContext ctx, string[] args)
    {
        System.Console.WriteLine($"Loadout count: {await ctx.Loadouts.CountAsync()}");
        System.Console.WriteLine($"Fighter count: {await ctx.Fighters.CountAsync()}");
        System.Console.WriteLine($"Card count: {await ctx.Cards.CountAsync()}");
    }
}