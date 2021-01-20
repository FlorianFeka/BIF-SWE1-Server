using SocketTry.Http;
using System.Net;

namespace MonsterTradingCardsGame
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 10001);
            using (var server = new HttpServer())
            {
                server.Start(localEndPoint);
            }
        }
    }
}
