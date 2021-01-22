using System;

namespace MonsterTradingCardsGame.Models.Cards
{
    public class CardsDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Damage { get; set; }
    }
}
