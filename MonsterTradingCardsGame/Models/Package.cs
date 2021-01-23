using System;

namespace MonsterTradingCardsGame.Models
{
    public class Package
    {
        public static readonly int PackageSize = 5;
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid[] Cards { get; } = new Guid[PackageSize];
    }
}
