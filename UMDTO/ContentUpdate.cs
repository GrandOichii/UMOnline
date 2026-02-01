using UMCore.Templates;

namespace UMDTO;

public class ContentUpdateGet
{
    public required string Core { get; init; }
    public required List<LoadoutTemplate> Loadouts { get; init; }
}