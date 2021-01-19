using SocketTry.Implementations;

namespace SocketTry
{
    public abstract class BaseController
    {
        internal HttpRequest HttpRequest { get; set; }
        internal HttpResponse HttpResponse { get; set; }
    }
}
