using System;
using System.Net.Sockets;
using System.Text;

namespace SimpleServer.Handler
{
    internal abstract class SocketHandler : IDisposable
    {
        private bool _listening = true;

        private Socket _socket;

        private byte[] _receiveBuffer;
        private int _receiveBufferSize;

        private byte[] _sendData;
        private int _sendDataSizeLeft;
        private int _sentDataSize;
        private int _sendBufferSize;

        internal SocketHandler(Socket socket, int receiveBufferSize, int sendBufferSize)
        {
            _socket = socket;
            _socket.NoDelay = true;
            _socket.Blocking = false;
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            _receiveBufferSize = receiveBufferSize;
            _sendBufferSize = sendBufferSize;
            _receiveBuffer = new byte[_receiveBufferSize];
        }

        protected void BeginReceive()
        {
            if (!_listening) return;
            try
            {
                if (_socket != null)
                {
                    _socket.BeginReceive(
                        _receiveBuffer, 0, _receiveBuffer.Length,
                        SocketFlags.None, new AsyncCallback(HandleReceive), null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }

        internal void HandleReceive(IAsyncResult result)
        {
            if (!_listening) return;
            int bytesRead = 0;
            if (_socket != null)
            {
                try
                {
                    bytesRead = _socket.EndReceive(result);
                } catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (bytesRead < 1 || bytesRead > _receiveBuffer.Length)
            {
                Dispose();
                return;
            }

            var recStr = Encoding.UTF8.GetString(_receiveBuffer);
            Console.WriteLine(recStr);


            Receive(_receiveBuffer);
            BeginReceive();
        }

        internal void Send(byte[] data)
        {
            _sendData = data;
            _sendDataSizeLeft = data.Length;
            BeginSend();
        }

        private void BeginSend()
        {
            var len = _sendDataSizeLeft;
            if(len > _sendBufferSize)
            {
                len = _sendBufferSize;
            }
            _socket.BeginSend(_sendData, _sentDataSize, len, SocketFlags.None, new AsyncCallback(HandleSend), null);
        }

        private void HandleSend(IAsyncResult result)
        {
            if (!_listening) return;
            try
            {
                int sentBytes = _socket.EndSend(result);
                _sendDataSizeLeft -= sentBytes;
                _sentDataSize += sentBytes;
                if (_sendDataSizeLeft == 0)
                {
                    ClearForNewSend();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Dispose();
            }
            BeginSend();
        }

        private void ClearForNewSend()
        {
            _sendData = null;
            _sendDataSizeLeft = 0;
            _sentDataSize = 0;
            _receiveBuffer = new byte[_receiveBufferSize];
        }

        public void Dispose()
        {
            _listening = false;
            if (_socket != null)
            {
                Console.WriteLine($"Client disconnected: {_socket.RemoteEndPoint}");
                _socket.Close(100);
            }
        }

        internal abstract void Receive(byte[] buffer);
    }
}
