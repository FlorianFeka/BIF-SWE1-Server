namespace SimpleServer.Attributes.Verbs
{
    public class HttpDeleteAttribute : HttpVerbAttribute
    {
        public HttpDeleteAttribute() { }
        public HttpDeleteAttribute(string sufixRoute)
        {
            SufixRoute = sufixRoute;
        }
    }
}
