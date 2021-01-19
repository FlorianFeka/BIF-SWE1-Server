using SocketTry.Implementations;

namespace SocketTry
{
    public abstract class BaseController
    {
        public HttpRequest HttpRequest { get; set; }
        public HttpResponse HttpResponse { get; set; }
    }
}
