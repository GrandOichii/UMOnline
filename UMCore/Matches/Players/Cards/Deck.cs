using System.Threading.Tasks;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public class Deck : MatchCardCollection
{
    public Deck(Player owner) : base(owner){}

    public async Task<List<MatchCard>> TakeFromTop(int amount)
    {
        var result = new List<MatchCard>();
        while (Count > 0 && amount-- > 0)
        {
            var card = Cards[0];
            result.Add(card);
            Cards.Remove(card);
        }
        if (amount > 0)
        {
            await Owner.Exhaust(amount);
        }
        return result;
    }

    public override Data GetData(Player player)
    {
        return new()
        {
            Cards = [],
            Count = Cards.Count,
        };
    }
}