using MonsterTradingCardsGame.Services;
using SocketTry.Http;
using SocketTry.Implementations;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Util
{
    public static class Utils
    {
        public static bool InvalidAuth(SessionService sessionService, HttpRequest request, HttpResponse response)
        {
            if (!request.Headers.TryGetValue(HttpMeta.Headers.AUTHORIZATION, out var token))
            {
                response.SetContent("No Authorization token!");
                response.SetStatus(HttpStatus.Bad_Request);
                return true;
            }
            if (!sessionService.ValidSession(token))
            {
                response.SetContent("Unauthorized!");
                response.SetStatus(HttpStatus.Unauthorized);
                return true;
            }
            return false;
        }
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
