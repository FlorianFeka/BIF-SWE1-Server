namespace SocketTry.Attributes.Verbs
{
    public class HttpPostAttribute : HttpVerbAttribute
    {
        public HttpPostAttribute() { }
        public HttpPostAttribute(string sufixRoute)
        {
            SufixRoute = sufixRoute;
        }
    }
}
