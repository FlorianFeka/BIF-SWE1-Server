using System;
using System.Net.Sockets;

namespace SocketTry.Handler
{
    public class HttpHandler : SocketHandler
    {
        public HttpHandler(Socket socket, int receiveBufferSize, int sendBufferSize) : base(socket, receiveBufferSize, sendBufferSize) { }

        public override bool Receive(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
