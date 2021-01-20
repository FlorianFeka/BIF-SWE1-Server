using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;

namespace MonsterTradingCardsGame.Services
{
    public class UserService
    {
        private UserRepository _userRepository;

        public UserService()
        {
            _userRepository = SingletonFactory.GetObject<UserRepository>();
        }

        public bool Test()
        {
            return _userRepository.TestThis();
        }

        public bool CreateUser(User user)
        {

            return true;
        }
    }
}
