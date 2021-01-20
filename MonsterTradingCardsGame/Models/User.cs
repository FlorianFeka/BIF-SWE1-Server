using System;

namespace MonsterTradingCardsGame.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }
}
