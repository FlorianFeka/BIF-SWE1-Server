using SocketTry.Handler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Linq;
using SocketTry.Attributes;

namespace SocketTry.Http
{
    public class HttpServer : IDisposable
    {
        private Socket _listener;
        private bool _listening = true;
        private Assembly _entryAssembly;

        private struct ControllerAttributesDto
        {
            public Type Type;
            public object[] Attributes;
        }

        private struct ControllerFunctionAttributeDto
        {
            public ControllerAttributesDto Controller;
            public MethodInfo MethodInfo;
            public object[] Attributes;
        }

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

            _entryAssembly = Assembly.GetEntryAssembly();
            LoadControllers();

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


        private void LoadControllers()
        {
            var controllers = FindControllers();
            var handlers = FindHttpHandlers(controllers);
        }

        private IEnumerable<ControllerFunctionAttributeDto> FindHttpHandlers(IEnumerable<ControllerAttributesDto> controllers)
        {
            return
                from controller in controllers
                from func in controller.Type.GetMethods()
                let attributes = func.GetCustomAttributes(typeof(HttpVerbAttribute), true)
                where attributes != null && attributes.Length > 0
                select new ControllerFunctionAttributeDto
                {
                    Controller = controller,
                    MethodInfo = func,
                    Attributes = attributes
                };

        }

        private IEnumerable<ControllerAttributesDto> FindControllers()
        {
            return
                from type in _entryAssembly.GetTypes()
                let attributes = type.GetCustomAttributes(typeof(ControllerDataAttribute), true)
                where attributes != null && attributes.Length > 0
                select new ControllerAttributesDto
                {
                    Type = type,
                    Attributes = attributes
                };
        }
    }
}
