namespace UMClient.Storage.Models;

public class CardModel
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required int DeckId { get; set; }
}