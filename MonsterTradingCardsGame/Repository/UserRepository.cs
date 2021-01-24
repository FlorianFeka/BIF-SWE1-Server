using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Repository
{
    public class UserRepository
    {
        private readonly DatabaseConnection _connection;

        public UserRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
        }

        public bool CreateUser(User user)
        {
            SqlCommand command = new SqlCommand(_createUserCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Id", SqlDbType.UniqueIdentifier, 16, user.Id));
            command.Parameters.Add(Utils.CreateSqlParameter("@Username", SqlDbType.VarChar, 255, user.Username));
            command.Parameters.Add(Utils.CreateSqlParameter("@Password", SqlDbType.VarChar, 255, user.Password));
            command.Parameters.Add(Utils.CreateSqlParameter("@Bio", SqlDbType.VarChar, 255, user.Bio != null ? user.Bio : ""));
            command.Parameters.Add(Utils.CreateSqlParameter("@Image", SqlDbType.VarChar, 255, user.Image != null ? user.Image : ""));
            command.Parameters.Add(Utils.CreateSqlParameter("@Money", SqlDbType.Int, 4, user.Money));
            command.Prepare();
            var rowsAdded = command.ExecuteNonQuery();
            return rowsAdded == 1;
        }

        public bool UserExists(string username)
        {
            SqlCommand command = new SqlCommand(_userExistsCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Username", SqlDbType.VarChar, 255, username));
            command.Prepare();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var count = reader[0] as int?;
                if (count.HasValue && count.Value > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public User GetUserWithUsername(string usernameParam)
        {
            User user = null;
            SqlCommand command = new SqlCommand(_selectUserWithUsernameCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Username", SqlDbType.VarChar, 255, usernameParam));
            command.Prepare();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    Id = reader.GetGuid(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Bio = reader.GetString(3),
                    Image = reader.GetString(4),
                    Money = reader.GetInt32(5),
                };
            }
            return user;
        }

        public User GetUserWithId(Guid userId)
        {
            SqlCommand command = new SqlCommand(_selectUserWithIdCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Id", SqlDbType.UniqueIdentifier, 16, userId));
            command.Prepare();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetGuid(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Bio = reader.GetString(3),
                    Image = reader.GetString(4),
                    Money = reader.GetInt32(5),
                };
            }
            return null;
        }

        private readonly string _createUserCommandString = "INSERT INTO [dbo].[Users] ([Id],[Username],[Password],[Bio],[Image],[Money])" +
            "VALUES (@Id,@Username,@Password,@Bio,@Image,@Money);";

        private readonly string _userExistsCommandString = "Select COUNT(*) FROM [dbo].[Users] WHERE [Username] = @Username";

        private readonly string _selectUserWithUsernameCommandString = "Select [Id], [Username], [Password], [Bio], [Image], [Money]" +
            "FROM[dbo].[Users] WHERE[Username] = @Username";

        private readonly string _selectUserWithIdCommandString = "Select [Id], [Username], [Password], [Bio], [Image], [Money]" +
            "FROM[dbo].[Users] WHERE[Id] = @Id";
    }
}
