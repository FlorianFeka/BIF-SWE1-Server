using SocketTry.Implementations;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SocketTry.Handler
{
    public class HttpHandler : IDisposable
    {
        private bool _listening = true;
        private Socket _socket;
        private byte[] _buffer;
        private string _leftOverContent;
        private HttpRequest _httpRequest = new HttpRequest();
        private MemoryStream _data = new MemoryStream();
        public HttpHandler(Socket socket, int bufferSize)
        {
            _socket = socket;
            _socket.Blocking = false;
            _buffer = new byte[bufferSize];
            BeginReceive();
        }

        private void BeginReceive()
        {
            if (_socket != null)
            {
                _socket.BeginReceive(
                    _buffer, 0, _buffer.Length,
                    SocketFlags.None, new AsyncCallback(HandleReceive), null);
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

            if (bytesRead > 0)
            {
                _data.Write(_buffer, 0, _buffer.Length);
                if (TryGetLinesFromChunk(_buffer, out var lines))
                {
                    if (_httpRequest.ParseChunk(lines, _leftOverContent))
                    {
                        string content = Encoding.ASCII.GetString(_data.ToArray());
                        Console.WriteLine(content);
                        var answer = "HTTP/1.1 200 OK\nConnection: keep-alive\n\n";
                        var a = Encoding.ASCII.GetBytes(answer);
                        _socket.BeginSend(a, 0, a.Length, SocketFlags.None,
                            new AsyncCallback(HandleSend), null);
                        return;
                    }
                }
            }
            else
            {
                if (_buffer.Length > 1)
                {
                    string content = Encoding.ASCII.GetString(_data.ToArray());
                    Console.WriteLine(content);
                }
                return;
            }
            BeginReceive();
        }

        private void HandleSend(IAsyncResult result)
        {
            _socket.EndSend(result);
            Dispose();
        }

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
