using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Util;
using System;
using System.Data;
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
            command.Parameters.Add(CreateSqlParameter("@Id", SqlDbType.UniqueIdentifier, 16, user.Id));
            command.Parameters.Add(CreateSqlParameter("@Username", SqlDbType.VarChar, 255, user.Username));
            command.Parameters.Add(CreateSqlParameter("@Password", SqlDbType.VarChar, 255, user.Password));
            command.Parameters.Add(CreateSqlParameter("@Bio", SqlDbType.VarChar, 255, user.Bio != null ? user.Bio : ""));
            command.Parameters.Add(CreateSqlParameter("@Image", SqlDbType.VarChar, 255, user.Image != null ? user.Image : ""));
            command.Prepare();
            var rowsAdded = command.ExecuteNonQuery();
            return rowsAdded == 1;
        }

        public bool UserExists(string username)
        {
            SqlCommand command = new SqlCommand(_userExistsCommandString, _connection.GetConnection());
            command.Parameters.Add(CreateSqlParameter("@Username", SqlDbType.VarChar, 255, username));
            command.Prepare();
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var count = reader[0] as int?;
                if (count.HasValue && count.Value > 0)
                {
                    return true;
                }
            }
            else
            {
                throw new Exception("Read for existing didn't have any rows (should have one)!");
            }
            reader.Close();
            return false;
        }

        private SqlParameter CreateSqlParameter(string parameterName, SqlDbType sqlDbType, int size, object value)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Size = size,
                Value = value
            };
        }

        public bool TestThis()
        {
            return true;
        }

        private string _insertCommandString = "INSERT INTO [dbo].[Users] ([Id],[Username],[Password],[Bio],[Image])" +
            "VALUES(@Id,@Username,@Password,@Bio,@Image);";
        private string _userExistsCommandString = "Select COUNT(*) FROM [dbo].[Users] WHERE [Username] = @Username";
    }
}
