using UMCore.Templates;

namespace UMModel.Models;

public class ContentUpdate
{
    public required int Id { get; set; }
    public required string Description { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedDT { get; set; }
    public required string Data { get; set; }
}