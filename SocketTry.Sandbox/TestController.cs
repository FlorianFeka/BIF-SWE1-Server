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
            return "test";
        }
    }
}
