using System;

namespace MonsterTradingCardsGame.Models.Sessions
{
    public class Session
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Guid UserId { get; set; }
    }
}
