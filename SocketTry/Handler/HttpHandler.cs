using SocketTry.Http;
using SocketTry.Implementations;
using SocketTry.Utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static SocketTry.Http.HttpServer;

namespace SocketTry.Handler
{
    internal class HttpHandler : SocketHandler
    {
        private string _leftOverContent;
        private HttpRequest _httpRequest = new HttpRequest();
        private Dictionary<HttpMethod, Dictionary<string, HttpVerbMethod>> _handlers;

        internal HttpHandler(Socket socket, int receiveBufferSize, int sendBufferSize, Dictionary<HttpMethod, Dictionary<string, HttpVerbMethod>> handlers) : base(socket, receiveBufferSize, sendBufferSize)
        {
            _handlers = handlers;
            BeginReceive();
        }

        internal override void Receive(byte[] buffer)
        {
            if (TryGetLinesFromChunk(buffer, out var lines))
            {
                try
                {
                    if (_httpRequest.ParseChunk(lines, _leftOverContent))
                    {
                        var response = GetHandlerResult();
                        var answer = response.ToString();
                        Console.WriteLine(answer);
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
                        ClearForNewRequest();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Dispose();
                }
            }
        }

        private HttpResponse GetHandlerResult()
        {
            HttpVerbMethod? handler = null;
            HttpResponse response = new HttpResponse();
            string routeSuffix = null;
            if (_httpRequest.Method.HasValue && !string.IsNullOrEmpty(_httpRequest.Url.Path))
            {
                handler = GetHttpMethodHandler(_httpRequest.Method.Value, _httpRequest.Url.Path, out routeSuffix);
            }
            if (handler.HasValue)
            {
                object result;
                var controllerInfo = handler.Value.ControllerType.GetConstructor(Type.EmptyTypes);
                var controllerObject = (controllerInfo.Invoke(new object[] { }) as BaseController);
                controllerObject.HttpRequest = _httpRequest;
                controllerObject.HttpResponse = response;
                object[] parameters;
                var parameterInfos = handler.Value.Method.GetParameters();
                if (handler.Value.HasRouteSuffix && handler.Value.HasBody)
                {
                    var castedRouteSuffix = Convert.ChangeType(routeSuffix, parameterInfos[0].ParameterType);
                    object castedBody = _httpRequest.ContentString;
                    if (parameterInfos[1].ParameterType != typeof(string))
                    {
                        castedBody = JsonSerializer.Deserialize(_httpRequest.ContentString, parameterInfos[1].ParameterType);
                    }

                    parameters = new object[] { castedRouteSuffix, castedBody };
                }
                else if (handler.Value.HasRouteSuffix)
                {
                    var castedRouteSuffix = Convert.ChangeType(routeSuffix, parameterInfos[0].ParameterType);
                    parameters = new object[] { castedRouteSuffix };
                }
                else if (handler.Value.HasBody)
                {
                    object castedBody = _httpRequest.ContentString;
                    if (parameterInfos[0].ParameterType != typeof(string))
                    {
                        castedBody = JsonSerializer.Deserialize(_httpRequest.ContentString, parameterInfos[0].ParameterType);
                    }
                    parameters = new object[] { castedBody };
                }
                else
                {
                    parameters = new object[] { };
                }
                result = handler.Value.Method.Invoke(controllerObject, parameters);
                if(result != null)
                {
                    if (!(result is string))
                    {
                        result = JsonSerializer.Serialize(result);
                    }
                }
                else
                {
                    result = "";
                }
                if(response.ContentBytes == null)
                {
                    response.SetContent((result as string));
                }
            }
            else
            {
                response.SetStatus(HttpStatus.Not_Found);
                response.Headers[HttpMeta.Headers.CONNECTION] = "Closed";

                response.SetContent($"Couldn't find route: {_httpRequest.Url.Path}");
            }

            return response;
        }

        private HttpVerbMethod? GetHttpMethodHandler(HttpMethod httpMethod, string path, out string routeSuffix)
        {
            routeSuffix = null;
            if (_handlers.TryGetValue(httpMethod, out var routesHandler))
            {
                string route = null;
                foreach (var routePath in routesHandler.Keys)
                {
                    if (routePath.Split("/").Length == path.Split("/").Length)
                    {
                        if(CheckPaths(routePath, path, out routeSuffix))
                        {
                            route = routePath;
                            break;
                        }
                    }
                }
                if (route != null && routesHandler.TryGetValue(route, out var handler))
                {
                    return handler;
                }
            }

            return null;
        }

        public bool CheckPaths(string route, string path, out string routeSuffix)
        {
            routeSuffix = null;
            if (path == route) return true;
            var pathParts = path.Split("/");
            var routeParts = route.Split("/");

            if (pathParts.Length != routeParts.Length) return false;

            for (int i = 0; i < routeParts.Length; i++)
            {
                var match = Util.RouteSuffixRegex.Match(routeParts[i]);
                if (match.Success)
                {
                    routeSuffix = pathParts[i];
                }
                else
                {
                    if (routeParts[i] != pathParts[i])
                    {
                        return false;
                    }
                }
            }

            return true;
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
