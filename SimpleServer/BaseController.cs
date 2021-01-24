using SimpleServer.Implementations;

namespace SimpleServer
{
    public abstract class BaseController
    {
        public HttpRequest HttpRequest { get; set; }
        public HttpResponse HttpResponse { get; set; }
    }
}
