namespace UMDTO;

public class CreateMatchParams
{
    public required string MatchConfigName { get; init; }
    public required string Title { get; init; }
    public required List<string> AllowedLoadouts { get; init; }
}