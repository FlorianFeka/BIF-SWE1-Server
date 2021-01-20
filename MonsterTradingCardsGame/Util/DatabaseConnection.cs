using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Util
{
    public class DatabaseConnection
    {
        public SqlConnection Connection { 
            get 
            {
                if(Connection != null && Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                    Connection.Open();
                }
                return Connection;
            }
            private set => Connection = value; }
        private static string _connectionString = "Data Source=.;Initial Catalog=MonsterTradingCardGame;User Id=sa;Password=veryLongPassword!";

        public DatabaseConnection()
        {
            Connection = new SqlConnection(_connectionString);
            Connection.Open();
        }

        ~DatabaseConnection(){
            Connection.Dispose();
        }
    }
}
