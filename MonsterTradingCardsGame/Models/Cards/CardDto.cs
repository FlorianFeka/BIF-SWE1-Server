using System;

namespace MonsterTradingCardsGame.Models.Cards
{
    public class CardDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Damage { get; set; }
    }
}
