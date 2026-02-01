using System.Text.Json.Serialization;
using UMCore.Templates;

namespace UMDTO;

public class ContentUpdateGet
{
    [JsonPropertyName("core")]
    public required string Core { get; init; }
    [JsonPropertyName("loadouts")]
    public required List<LoadoutTemplate> Loadouts { get; init; }
}