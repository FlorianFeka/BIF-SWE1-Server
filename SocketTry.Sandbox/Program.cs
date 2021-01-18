using SocketTry.Http;
using System;
using System.Net;

namespace SocketTry.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8081);
            using (var server = new HttpServer())
            {
                server.Start(localEndPoint);
                Console.WriteLine("\nPress [Enter] to Stop the Server!\n");
                Console.ReadLine();
            }
        }
    }
}
