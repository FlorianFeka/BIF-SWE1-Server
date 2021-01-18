using SocketTry.Implementations;
using SocketTry.Utils;
using System;
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

        private string _leftOverContent;
        private HttpRequest _httpRequest = new HttpRequest();

        private int _sendRound = 0;
        private byte[] _sendData;
        private int _sendBufferSize;

        private readonly byte _endOfString = (byte)'\0';

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

            var recStr = Encoding.ASCII.GetString(_receiveBuffer);
            Console.WriteLine(recStr);


            //if (Receive(_receiveBuffer)) BeginReceive();

            if (TryGetLinesFromChunk(_receiveBuffer, out var lines))
            {
                try
                {
                    if (_httpRequest.ParseChunk(lines, _leftOverContent))
                    {
                        var answer = "HTTP/1.1 200 OK\nConnection: keep-alive\nContent-Type: text/html\n";
                        var co = "<h1>Test</h1>";
                        answer += $"Content-Length: {co.Length}\n\n{co}";
                        var a = Encoding.ASCII.GetBytes(answer);
                        try
                        {
                            //_socket.BeginSend(a, 0, a.Length, SocketFlags.None,
                            //new AsyncCallback(HandleSend), null);
                            Send(a);
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
            _sendData = data;
            SendBuffer();
        }

        private void SendBuffer()
        {
            try
            {
                var lastPart = CreateSendBuffer(out var buffer);
                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(HandleSend), lastPart);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// Creates a byte array which is part of the data to be sent
        /// and fills the empty space if there is any with \0 (end of string)
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>returns <c>true</c> if the created buffer is the last one to be created, otherwise it returns <c>false</c></returns>
        /// <exception cref="Exception">throws exception when the configured send_buffer_size is bellow 1</exception>
        private bool CreateSendBuffer(out byte[] buffer)
        {
            if (_sendBufferSize < 1) throw new Exception("Too small send_buffer!");

            int from = _sendBufferSize * _sendRound;
            from = from != 0 ? from++ : 0;
            if (from >= _sendData.Length) throw new Exception("Data should already be sent!");

            var len = _sendBufferSize;
            len = len < _sendData.Length ? len : _sendData.Length - from;

            buffer = new byte[_sendBufferSize];

            Array.Copy(_sendData, from, buffer, 0, len);

            if (len < _sendBufferSize) Util.FillEmptySpaceWith<byte>(buffer, _endOfString, len);

            _sendRound++;
            return (from + len) == _sendData.Length;
        }

        private void HandleSend(IAsyncResult result)
        {
            if (!_listening) return;
            try
            {
                _socket.EndSend(result);
                var lastPart = result.AsyncState as bool?;
                if (lastPart.HasValue && lastPart.Value)
                {
                    ClearForNewRequest();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Dispose();
            }
            SendBuffer();
        }

        private void ClearForNewRequest()
        {
            _leftOverContent = "";
            _httpRequest = new HttpRequest();

            _sendData = null;
            _sendRound = 0;
        }

        /// <returns>returns <c>true</c> if to continue receiving, otherwise returns <c>false</c></returns>
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
