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
            SqlCommand command = new SqlCommand(_insertCommandString, _connection.GetConnection());
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Bio", user.Bio != null ? user.Bio : "");
            command.Parameters.AddWithValue("@Image", user.Image != null ? user.Image : "");
            var rowsAdded = command.ExecuteNonQuery();
            return rowsAdded == 1;
        }

        public bool TestThis()
        {
            return true;
        }

        private string _insertCommandString = "INSERT INTO [dbo].[Users] ([Id],[Name],[Password],[Bio],[Image])" +
            "VALUES(@Id,@Username,@Password,@Bio,@Image);";
    }
}
