namespace UMDTO;

public class ChatMessage
{
    public required string From { get; init; }
    public required string Msg { get; init; }
    public required string MatchId { get; init; }
}