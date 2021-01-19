using SocketTry.Attributes;
using SocketTry.Handler;
using SocketTry.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SocketTry.Http
{
    public class HttpServer : IDisposable
    {
        private Socket _listener;
        private IPEndPoint _endPoint;
        private bool _listening = true;
        private Assembly _entryAssembly;

        private Dictionary<HttpMethod, Dictionary<string, HttpVerbMethod>> _handlers = new Dictionary<HttpMethod, Dictionary<string, HttpVerbMethod>>();

        internal struct HttpVerbMethod
        {
            public Type ControllerType;
            public MethodInfo Method;
            public bool HasRouteSuffix;
            public bool HasBody;
        }

        private struct ControllerAttributesDto
        {
            public Type Type;
            public object[] Attributes;
        }

        private struct ControllerFunctionAttributeDto
        {
            public MethodInfo MethodInfo;
            public HttpVerbAttribute HttpAttribute;
        }

        public void Dispose()
        {
            _listening = false;
            _listener.Close();
        }

        public void Start(IPEndPoint endPoint)
        {
            _endPoint = endPoint;

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.NoDelay = true;
            _listener.Blocking = false;
            _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _listener.Bind(endPoint);
            _listener.Listen(100);

            _entryAssembly = Assembly.GetEntryAssembly();
            LoadControllers();
        }

        private void LoadControllers()
        {
            var controllers = FindControllers();
            //var handlers = FindHttpHandlers(controllers);

            foreach (var controller in controllers)
            {
                var routeAttribute = controller.Attributes.Where(x => x is RouteAttribute).FirstOrDefault() as RouteAttribute;
                if(routeAttribute == null)
                {
                    var controllerName = controller.Type.Name;
                    throw new Exception($"Controller [{controllerName}] without Route!");
                }
                var baseRoute = routeAttribute.Route;
                if (!baseRoute.StartsWith("/")) baseRoute = "/" + baseRoute;
                var httpMethods = GetHttpMethods(controller.Type);
                LoadHttpFunctions(controller.Type, baseRoute, httpMethods);

            }

            StartFirstAccept();
        }

        private void LoadHttpFunctions(Type type, string baseRoute, IEnumerable<ControllerFunctionAttributeDto> methodInfos)
        {
            foreach (var methodInfo in methodInfos)
            {
                var route = baseRoute;
                var httpMethod = GetHttpMethod(methodInfo.HttpAttribute);
                var routeSuffix = methodInfo.HttpAttribute.SufixRoute;
                var hasRouteSuffix = false;
                if (!string.IsNullOrEmpty(routeSuffix))
                {
                    // not really a good solution since one could still put anything in there
                    // TODO: if I got the time make a regex for this check
                    if (routeSuffix.StartsWith("/")) routeSuffix = routeSuffix.Substring(1);
                    ValidateSuffixRoute(methodInfo.MethodInfo, routeSuffix);
                    hasRouteSuffix = true;
                    route = $"{baseRoute}/{routeSuffix}";
                }
                var hasBody = HasBody(methodInfo.MethodInfo, hasRouteSuffix);
                AddHandler(type, httpMethod, route, methodInfo.MethodInfo, hasRouteSuffix, hasBody);
            }
        }

        private void AddHandler(Type type, HttpMethod httpMethod, string route, MethodInfo methodInfo, bool hasRouteSuffix, bool hasBody)
        {
            if (!_handlers.TryGetValue(httpMethod, out var routes))
            {
                _handlers.Add(httpMethod, routes = new Dictionary<string, HttpVerbMethod>());
            }

            var duplicate = routes.Keys
                .Where(x => Util.GetPurePath(x) == Util.GetPurePath(route))
                .FirstOrDefault();

            if (duplicate != null)
            {
                throw new Exception($"Duplicate route: '{route}' - '{duplicate}'");
            }

            routes.Add(route, new HttpVerbMethod
            {
                ControllerType = type,
                Method = methodInfo,
                HasRouteSuffix = hasRouteSuffix,
                HasBody = hasBody,
            });
        }

        private HttpVerbMethod? GetHttpMethodHandler(HttpMethod httpMethod, string path)
        {
            if (_handlers.TryGetValue(httpMethod, out var routesHandler))
            {
                var route = routesHandler.Keys
                    .Where(x =>
                    {
                        if (x.Split("/").Length == path.Split("/").Length)
                        {
                            return Util.GetPurePath(x) == Util.GetPurePath(path);
                        }
                        return false;
                    })
                    .FirstOrDefault();
                if (route != null && routesHandler.TryGetValue(route, out var handler))
                {
                    return handler;
                }
            }

            return null;
        }

        private bool HasBody(MethodInfo methodInfo, bool hasRouteSuffix)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 2 || (!hasRouteSuffix && parameters.Length > 1)) throw new Exception($"Method [{methodInfo.Name}] has too many functions!");
            if (hasRouteSuffix && parameters.Length == 1) return false;

            return true;
        }

        private void ValidateSuffixRoute(MethodInfo methodInfo, string suffixRoute)
        {
            var test = "{whatATest}";
            Console.WriteLine(test.Substring(1, test.Length - 2));
            var firstParameter = methodInfo.GetParameters().FirstOrDefault();
            if (firstParameter == null || firstParameter.Name != suffixRoute.Substring(1, suffixRoute.Length - 2))
            {
                throw new Exception($"No Parameter for Suffix Route in method [{methodInfo.Name}]!");
            }
        }

        private HttpMethod GetHttpMethod(HttpVerbAttribute httpVerbAttribute)
        {
            if(httpVerbAttribute is HttpGetAttribute)
            {
                return HttpMethod.GET;
            }

            throw new Exception("Unknown HttpVerbAttribute!");
        }

        private IEnumerable<ControllerFunctionAttributeDto> GetHttpMethods(Type type)
        {
            return
                from methodInfo in type.GetMethods()
                let attribute = methodInfo.GetCustomAttribute(typeof(HttpVerbAttribute))
                where attribute != null
                select new ControllerFunctionAttributeDto
                {
                    MethodInfo = methodInfo,
                    HttpAttribute = (attribute as HttpVerbAttribute)
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

        internal void StartFirstAccept()
        {
            Console.WriteLine($"Server listening on {_endPoint}");
            BeginAccept();
        }

        internal void BeginAccept()
        {
            _listener.BeginAccept(HandleAccept, null);
        }

        internal void HandleAccept(IAsyncResult result)
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
