using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Util;
using System;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Repository
{
    public class UserRepository
    {
        private DatabaseConnection _connection;

        public UserRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
        }

        public bool CreateUser(User user)
        {

            return true;
        }

        public bool TestThis()
        {
            return true;
        }
    }
}
