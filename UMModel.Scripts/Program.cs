using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UMModel;
using UMModel.Scripts;
using UMModel.Scripts.Scripts;

var scriptMap = new Dictionary<string, IScript>()
{
    { "TestScript", new TestScript() },
    { "ImportLoadouts", new ImportLoadouts() },
    { "UpdateCoreScript", new UpdateCoreScript() },
    { "SetPublicLoadouts", new SetPublicLoadouts() },
    { "CreateContentUpdate", new CreateContentUpdate() },
    { "Migrate", new Migrate() },
};

var scriptName = args[0];
if (scriptName is null)
{
    System.Console.WriteLine("Provide a script name");
    return;
}

if (!scriptMap.TryGetValue(scriptName, out var script))
{
    System.Console.WriteLine($"Unknown script name: {scriptName}");
    return;
}

await script.Run(new UMContext(), args);