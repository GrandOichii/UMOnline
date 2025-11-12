namespace UMModel.Models;

public class CoreScript
{
    public int? Id { get; set; } = null;
    public required DateTime CreatedAt { get; set; }
    public required string Script { get; set; }
    public required bool IsActive { get; set; }
}