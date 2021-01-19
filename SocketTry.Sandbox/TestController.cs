using SocketTry.Attributes;

namespace SocketTry.Sandbox
{
    [Route("/api")]
    [Controller]
    public class TestController : BaseController
    {
        [HttpGet("{id}")]
        public string GetTest(int id, string body)
        {
            return body + " ID: " + id;
        }
    }
}
