using System.Threading.Tasks;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public class Deck : MatchCardCollection
{
    public Deck(Player owner) : base(owner, "DECK") { }

    public override Data GetData(Player player)
    {
        return new()
        {
            Cards = [],
            Count = Cards.Count,
        };
    }    
}