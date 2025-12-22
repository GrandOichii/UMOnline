namespace UMClient.Storage.Models;

public class ScriptNodeModel
{
    public required int Id { get; set; }
    public required int Name { get; set; }
    public required int EditorOffsetX { get; set; }
    public required int EditorOffsetY { get; set; }
    public required int ScriptId { get; set; }
}