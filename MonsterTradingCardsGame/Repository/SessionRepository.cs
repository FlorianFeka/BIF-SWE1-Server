using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Models.Sessions;
using MonsterTradingCardsGame.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Repository
{
    public class SessionRepository
    {
        private DatabaseConnection _connection;
        private UserRepository _userRepository;
        private int _expiryMinutes = 30;

        public SessionRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
            _userRepository = SingletonFactory.GetObject<UserRepository>();
        }

        public bool CheckSession(string token)
        {
            SqlCommand command = new SqlCommand(_getSessionWithToken, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Token", SqlDbType.VarChar, 255, token));
            command.Prepare();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var session = new Session
                {
                    Id = reader.GetGuid(0),
                    Token = reader.GetString(1),
                    ExpiryDate = reader.GetDateTime(2),
                    UserId = reader.GetGuid(3),
                };
                if(DateTime.Now < session.ExpiryDate)
                {
                    return true;
                }
            }
            return false;
        }

        public Session SaveSession(User user)
        {
            var session = GetSessionWithUserId(user.Id);
            if(session != null)
            {
                return UpdateSession(session);
            }
            else
            {
                return CreateSession(user);
            }
        }

        public Session GetSessionWithUserId(Guid userId)
        {
            Session session = null;
            SqlCommand command = new SqlCommand(_getSessionWithUserId, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16, userId));
            command.Prepare();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                session = new Session
                {
                    Id = reader.GetGuid(0),
                    Token = reader.GetString(1),
                    ExpiryDate = reader.GetDateTime(2),
                    UserId = reader.GetGuid(3),
                };
            }
            return session;
        }

        private Session CreateSession(User user)
        {
            var session = new Session
            {
                Token = $"Basic {user.Username}-mtcgToken",
                ExpiryDate = DateTime.Now.AddMinutes(_expiryMinutes),
                UserId = user.Id
            };
            SqlCommand command = new SqlCommand(_createSessionCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Id", SqlDbType.UniqueIdentifier, 16, session.Id));
            command.Parameters.Add(Utils.CreateSqlParameter("@Token", SqlDbType.VarChar, 255, session.Token));
            command.Parameters.Add(Utils.CreateSqlParameter("@ExpiryDateTime", SqlDbType.DateTime, 8, session.ExpiryDate));
            command.Parameters.Add(Utils.CreateSqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16, session.UserId));
            command.Prepare();
            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 1)
            {
                return session;
            }
            return null;
        }

        private Session UpdateSession(Session session)
        {
            var updateExpiryDate = DateTime.Now.AddMinutes(_expiryMinutes);
            SqlCommand command = new SqlCommand(_updateSessionCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@Id", SqlDbType.UniqueIdentifier, 16, session.Id));
            command.Parameters.Add(Utils.CreateSqlParameter("@ExpiryDateTime", SqlDbType.DateTime, 8, updateExpiryDate));
            command.Prepare();
            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 1)
            {
                session.ExpiryDate = updateExpiryDate;
                return session;
            }
            return null;
        }

        private readonly string _createSessionCommandString = "INSERT INTO [dbo].[Sessions]([Id],[Token],[ExpiryDateTime],[UserId])" +
            "VALUES(@Id,@Token,@ExpiryDateTime,@UserId)";
        private readonly string _getSessionWithUserId = "SELECT [Id],[Token],[ExpiryDateTime],[UserId]" +
            "FROM [dbo].[Sessions] WHERE [UserId] = @UserId";
        private readonly string _updateSessionCommandString = "UPDATE [dbo].[Sessions] SET [ExpiryDateTime] = @ExpiryDateTime WHERE Id = @Id";
        private readonly string _getSessionWithToken = "SELECT [Id],[Token],[ExpiryDateTime],[UserId]" +
            "FROM [dbo].[Sessions] WHERE [Token] = @Token";
    }
}
