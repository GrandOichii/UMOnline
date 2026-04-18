namespace UMCore.Tests.Core;

public class ScriptLoader
{
    private readonly string _path;

    public ScriptLoader(string path = "../../../../")
    {
        _path = path;
    }

    public string Get(string name)
    {
        return File.ReadAllText(string.Join(_path, name));
    }
}