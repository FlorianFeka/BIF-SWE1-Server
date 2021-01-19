using SocketTry.Attributes;

namespace SocketTry.Sandbox
{
    [Route("/api")]
    [Controller]
    public class TestController : BaseController
    {

        [HttpGet]
        public string GetTest()
        {
            return "Empty Test Route";
        }

        [HttpGet("{id}")]
        public string GetTestWith(string id, string body)
        {
            return body + " ID: " + id;
        }
    }
}
