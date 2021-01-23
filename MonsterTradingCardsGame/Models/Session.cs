using System;
using System.Text.Json.Serialization;

namespace MonsterTradingCardsGame.Models
{
    public class Session
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
