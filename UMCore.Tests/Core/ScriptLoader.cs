namespace UMCore.Tests.Core;

public class ScriptLoader
{
    private readonly string _path;

    public ScriptLoader(string path = "../../../Core/Scripts")
    {
        _path = path;
    }

    public string Get(string name)
    {
        var p = System.IO.Path.Join(_path, name);
        return File.ReadAllText($"{p}.lua");
    }
}