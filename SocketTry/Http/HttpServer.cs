using SocketTry.Handler;
using System;
using System.Net;
using System.Net.Sockets;

namespace SocketTry.Http
{
    public class HttpServer : IDisposable
    {
        private Socket _listener;
        private bool _listening = true;

        public void Dispose()
        {
            _listening = false;
            _listener.Close();
        }

        public void Start(IPEndPoint endPoint)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.NoDelay = true;
            _listener.Blocking = false;
            _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
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
            if (!_listening) return;
            try
            {
                Socket socket = _listener.EndAccept(result);
                Console.WriteLine($"Accepted Connection from {socket.RemoteEndPoint}");

                new HttpHandler(socket, 4096, 4096);

                BeginAccept();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
