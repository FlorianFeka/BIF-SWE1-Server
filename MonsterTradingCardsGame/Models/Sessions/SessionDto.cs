using System;

namespace MonsterTradingCardsGame.Models.Sessions
{
    public class SessionDto
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
