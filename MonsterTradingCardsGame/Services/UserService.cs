using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;
using System;

namespace MonsterTradingCardsGame.Services
{
    public class UserService
    {
        private UserRepository _userRepository;

        public UserService()
        {
            _userRepository = SingletonFactory.GetObject<UserRepository>();
        }

        public User CheckPassword(string username, string password)
        {
            User user = _userRepository.GetUserWithUsername(username);
            if(user == null)
            {
                throw new Exception("User not found");
            }
            if(user.Password != password)
            {
                return null;
            }
            return user;
        }

        public bool CreateUser(User user)
        {
            return _userRepository.CreateUser(user);
        }

        public bool UserExists(string username)
        {
            return _userRepository.UserExists(username);
        }
    }
}
