using MonsterTradingCardsGame.Models.Types;
using System;
using System.Linq;

namespace MonsterTradingCardsGame.Models.Cards
{
    public class Card
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Damage { get; set; }
        public MonsterType? Monster { get; set; }
        public ElementType Element { get; set; } = ElementType.Normal;
        public bool IsSpell { get; set; }
        public Guid UserId { get; set; }

        public void SetTypes()
        {
            if (Name == null) throw new ArgumentNullException(nameof(Name));
            if (Name.Contains("Spell", StringComparison.OrdinalIgnoreCase))
            {
                IsSpell = true;
            }
            if (!IsSpell)
            {
                var monsterTypes = Enum.GetValues(typeof(MonsterType))
                    .Cast<MonsterType>()
                    .ToList();

                foreach (var monsterType in monsterTypes)
                {
                    if (Name.Contains(monsterType.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Monster = monsterType;
                        break;
                    }
                }
            }

            var elementTypes = Enum.GetValues(typeof(ElementType))
                .Cast<ElementType>()
                .ToList();

            foreach (var elementType in elementTypes)
            {
                if (Name.Contains(elementType.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    Element = elementType;
                    break;
                }
            }
        }

    }
}
