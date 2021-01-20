using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;

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

        [HttpPost]
        public string PostSomething()
        {
            return "Just a test post";
        }
    }
}
