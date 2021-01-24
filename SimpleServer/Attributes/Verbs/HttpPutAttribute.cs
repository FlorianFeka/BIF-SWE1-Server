namespace SimpleServer.Attributes.Verbs
{
    public class HttpPutAttribute : HttpVerbAttribute
    {
        public HttpPutAttribute() { }
        public HttpPutAttribute(string sufixRoute)
        {
            SufixRoute = sufixRoute;
        }
    }
}
