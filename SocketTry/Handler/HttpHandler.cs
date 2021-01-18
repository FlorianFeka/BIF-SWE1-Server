﻿using SocketTry.Implementations;
using System;
using System.Net.Sockets;
using System.Text;

namespace SocketTry.Handler
{
    public class HttpHandler : SocketHandler
    {
        private string _leftOverContent;
        private HttpRequest _httpRequest = new HttpRequest();

        public HttpHandler(Socket socket, int receiveBufferSize, int sendBufferSize) : base(socket, receiveBufferSize, sendBufferSize) { }

        public override void Receive(byte[] buffer)
        {
            if (TryGetLinesFromChunk(buffer, out var lines))
            {
                try
                {
                    if (_httpRequest.ParseChunk(lines, _leftOverContent))
                    {
                        ClearForNewRequest();
                        var answer = "HTTP/1.1 200 OK\nConnection: keep-alive\nContent-Type: text/html\n";
                        var co = "<h1>Test</h1>";
                        answer += $"Content-Length: {co.Length}\n\n{co}";
                        var a = Encoding.ASCII.GetBytes(answer);
                        try
                        {
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

        private void ClearForNewRequest()
        {
            _leftOverContent = "";
            _httpRequest = new HttpRequest();
        }
    }
}
