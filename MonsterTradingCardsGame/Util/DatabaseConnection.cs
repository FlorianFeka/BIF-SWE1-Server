using System;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Util
{
    public class DatabaseConnection
    {
        private SqlConnection _connection { get; set; }
        private static string _connectionString = "Data Source=.;Initial Catalog=MonsterTradingCardGame;User Id=sa;Password=veryLongPassword!";

        public DatabaseConnection()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public SqlConnection GetConnection()
        {
            if (_connection != null && _connection.State != ConnectionState.Open)
            {
                _connection.Close();
                _connection.Open();
            }
            return _connection;
        }

        ~DatabaseConnection(){
            Console.WriteLine("Dispose DatabaseConnection");
            _connection.Dispose();
        }
    }
}
