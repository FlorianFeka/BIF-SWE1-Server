using MonsterTradingCardsGame.Models.Types;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace MonsterTradingCardsGame.Models
{
    public class Card
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Damage { get; set; }

        [JsonIgnore]
        public MonsterType? Monster { get; set; }
        [JsonIgnore]
        public ElementType Element { get; set; } = ElementType.Normal;
        [JsonIgnore]
        public bool IsSpell { get; set; }
        [JsonIgnore]
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
