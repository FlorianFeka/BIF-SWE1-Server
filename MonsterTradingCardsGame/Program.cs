using SimpleServer.Http;
using System;
using System.Net;
using System.Text;

namespace MonsterTradingCardsGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 10001);
            using (var server = new HttpServer())
            {
                server.Start(localEndPoint);
            }
        }
    }
}
