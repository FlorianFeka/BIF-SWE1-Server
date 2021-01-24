using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using SimpleServer.Http;
using SimpleServer.Implementations;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Util
{
    public static class Utils
    {
        public static Session GetSession(SessionService sessionService, HttpRequest request, HttpResponse response)
        {
            if (!request.Headers.TryGetValue(HttpMeta.Headers.AUTHORIZATION, out var token))
            {
                response.SetContent("No Authorization token!");
                response.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            var session = sessionService.ValidSession(token);
            if (session == null)
            {
                response.SetContent("Unauthorized!");
                response.SetStatus(HttpStatus.Unauthorized);
                return null;
            }
            return session;
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
