using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Util
{
    public static class Utils
    {
        public static SqlParameter CreateSqlParameter(string parameterName, SqlDbType sqlDbType, int size, object value)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Size = size,
                Value = value
            };
        }
    }
}
