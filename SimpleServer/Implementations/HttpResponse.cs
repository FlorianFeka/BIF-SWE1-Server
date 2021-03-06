﻿using SimpleServer.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using SimpleServer.Http;
using System;
using System.Globalization;

namespace SimpleServer.Implementations
{
    public class HttpResponse : IResponse
    {
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>()
        {
            { HttpMeta.Headers.DATE, DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + " GMT + 1" },
            { HttpMeta.Headers.SERVER, "Some_random_server/0.1" },
            { HttpMeta.Headers.CONNECTION, "keep-alive" }
        }; // defaults

        public int ContentLength { get => ContentBytes.Length; }

        public string ContentType { get; set; }

        public byte[] ContentBytes { get; set; }

        public int StatusCode { get; set; } = 200; //default

        public string Status { get; set; } = "OK"; //default

        public string ServerHeader { get; set; }

        public void SetStatus(int code)
        {
            if(HttpMeta.CodeToStatus.TryGetValue(code, out var status))
            {
                StatusCode = code;
                Status = status;
                return;
            }
            throw new Exception("Invalid status code");
        }
        public void SetStatus(HttpStatus status)
        {
            if (HttpMeta.StatusToCode.TryGetValue(status, out var code))
            {
                StatusCode = code;
                var statusString = status.ToString()
                    .Replace("___", "'")
                    .Replace("__", "-")
                    .Replace("_", " ");
                Status = statusString;
                return;
            }
            throw new Exception("Invalid status");
        }

        public void AddHeader(string header, string value)
        {
            Headers[header] = value;
        }

        public void Send(NetworkStream stream)
        {
            SendFirstLine(stream);
            if(Headers.Count > 0)
            {
                SendHeaders(stream);
            }
            if(Headers.ContainsKey(HttpMeta.Headers.CONTENT_LENGTH))
            {
                SendContent(stream);
            }
            stream.Flush();
        }

        private void SendFirstLine(NetworkStream stream)
        {
            var line = $"{HttpMeta.VERSION} {StatusCode} {Status.ToUpper()}\n";
            var bytes = Encoding.UTF8.GetBytes(line);
            stream.Write(bytes);
        }

        private void SendHeaders(NetworkStream stream)
        {
            var stringBuilder = new StringBuilder();
            if (ContentBytes?.Length > 0)
            {
                Headers.TryAdd(HttpMeta.Headers.CONTENT_LENGTH, ContentBytes.Length.ToString());
            }
            foreach (var header in Headers)
            {
                stringBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
            stringBuilder.AppendLine();
            var headerBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            stream.Write(headerBytes, 0, headerBytes.Length);
        }

        private void SendContent(NetworkStream stream)
        {
            if(ContentBytes?.Length > 0)
            {
                stream.Write(ContentBytes, 0, ContentBytes.Length);
            }
        }

        public void SetContent(string content)
        {
            ContentBytes = Encoding.UTF8.GetBytes(content);
        }

        public void SetContent(byte[] content)
        {
            ContentBytes = content;
        }

        public void SetContent(Stream stream)
        {
            var mem = new MemoryStream();
            stream.CopyTo(mem);
            ContentBytes = mem.ToArray();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                string.Join(" ",
                    HttpMeta.VERSION, 
                    StatusCode.ToString(), 
                    Status.ToUpper()));

            if (ContentBytes?.Length > 0)
            {
                Headers[HttpMeta.Headers.CONTENT_LENGTH] = ContentBytes.Length.ToString();
            }
            foreach (var header in Headers)
            {
                stringBuilder.AppendLine(string.Join(": ", header.Key, header.Value));
            }

            stringBuilder.AppendLine();

            if (ContentBytes?.Length > 0)
            {
                stringBuilder.AppendLine(Encoding.UTF8.GetString(ContentBytes));
            }

            return stringBuilder.ToString();
        }
    }
}
