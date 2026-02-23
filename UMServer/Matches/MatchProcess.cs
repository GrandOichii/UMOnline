using UMDTO;

namespace UMServer.Matches;

public class MatchProcess(
    string id,
    CreateMatchParams createParams
)
{
    public string Id { get; } = id;
    public CreateMatchParams CreateParams { get; } = createParams;

    
    // TODO
}