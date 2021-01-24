using System;

namespace MonsterTradingCardsGame.Models
{
    public class UserDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Money { get; set; }

        public UserDto(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Bio = user.Bio;
            Image = user.Image;
            Money = user.Money;
        }
    }
}
