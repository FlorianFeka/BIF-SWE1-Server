using SocketTry.Implementations;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SocketTry.Handler
{
    public abstract class SocketHandler : IDisposable
    {
        private bool _listening = true;

        private Socket _socket;

        private byte[] _receiveBuffer;
        private int _receiveBufferSize;


        private MemoryStream _receiveData = new MemoryStream();
        private string _leftOverContent;
        private HttpRequest _httpRequest = new HttpRequest();

        private string _sendData;
        private byte[] _sendBuffer;
        private int _sendBufferSize;

        public SocketHandler(Socket socket, int receiveBufferSize, int sendBufferSize)
        {
            _socket = socket;
            _socket.NoDelay = true;
            _socket.Blocking = false;
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            _receiveBufferSize = receiveBufferSize;
            _sendBufferSize = sendBufferSize;
            _receiveBuffer = new byte[_receiveBufferSize];

            BeginReceive();
        }

        private void BeginReceive()
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

        public void HandleReceive(IAsyncResult result)
        {
            if (!_listening) return;
            int bytesRead = 0;
            if (_socket != null)
            {
                bytesRead = _socket.EndReceive(result);
            }

            if (bytesRead < 1 || bytesRead > _receiveBuffer.Length)
            {
                Dispose();
                return;
            }

            _receiveData.Write(_receiveBuffer, 0, _receiveBuffer.Length);
            var recStr = Encoding.ASCII.GetString(_receiveBuffer);
            Console.WriteLine(recStr);


            //Receive(_receiveBuffer);

            if (TryGetLinesFromChunk(_receiveBuffer, out var lines))
            {
                try
                {
                    if (_httpRequest.ParseChunk(lines, _leftOverContent))
                    {
                        string content = Encoding.ASCII.GetString(_receiveData.ToArray());
                        //Console.WriteLine(content);
                        var answer = "HTTP/1.1 200 OK\nConnection: keep-alive\nContent-Type: text/plain\n";
                        var co = "<h1>Test</h1>";
                        answer += $"Content-Length: {co.Length}\n\n{co}";
                        var a = Encoding.ASCII.GetBytes(answer);
                        try
                        {
                            _socket.BeginSend(a, 0, a.Length, SocketFlags.None,
                            new AsyncCallback(HandleSend), null);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Dispose();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Dispose();
                }
            }

            BeginReceive();
        }

        public void Send(byte[] data)
        {
            _sendBuffer = data;
            SendBuffer();
        }

        private void SendBuffer()
        {
            _socket.BeginSend(_sendBuffer, 0, _sendBufferSize, SocketFlags.None, new AsyncCallback(HandleSend), null);
        }

        private void HandleSend(IAsyncResult result)
        {
            ClearForNewRequest();
            if (!_listening) return;
            try
            {
                _socket.EndSend(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }

        private void ClearForNewRequest()
        {
            _leftOverContent = "";
            _httpRequest = new HttpRequest();
            _receiveData = new MemoryStream();
        }

        public abstract bool Receive(byte[] buffer);

        private bool TryGetLinesFromChunk(byte[] buffer, out string[] usableDataLines)
        {
            var chunkData = Encoding.ASCII.GetString(buffer);
            chunkData = chunkData.Replace("\r", "");
            chunkData = chunkData.Replace("\0", "");

            var content = _leftOverContent + chunkData;
            var positionOfLastNewLine = content.LastIndexOf("\n");
            usableDataLines = null;

            if (positionOfLastNewLine == -1) return false;

            var usableData = content.Substring(0, positionOfLastNewLine);
            usableDataLines = usableData.Split("\n");
            _leftOverContent = content.Substring(positionOfLastNewLine + 1);
            return true;
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
    }
}
