using System;

namespace MonsterTradingCardsGame.Models
{
    public class Deck
    {

        public static readonly int DeckSize = 4;
        public Guid UserId { get; set; }
        public Guid[] Cards { get; } = new Guid[DeckSize];
    }
}
