using SocketTry.Handler;
using System;
using System.Net;
using System.Net.Sockets;

namespace SocketTry.Http
{
    public class HttpServer : IDisposable
    {
        private Socket _listener;

        public void Dispose()
        {
            _listener.Close();
        }

        public void Start(IPEndPoint endPoint)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(endPoint);
            _listener.Listen(100);

            Console.WriteLine($"Server listening on {endPoint}");
            BeginAccept();
        }

        public void BeginAccept()
        {
            _listener.BeginAccept(HandleAccept, null);
        }

        public void HandleAccept(IAsyncResult result)
        {
            Socket socket = _listener.EndAccept(result);
            socket.NoDelay = true;
            socket.Blocking = false;
            Console.WriteLine($"Accepted Connection from {socket.RemoteEndPoint}");

            new HttpHandler(socket, 4096);

            BeginAccept();
        }
    }
}
