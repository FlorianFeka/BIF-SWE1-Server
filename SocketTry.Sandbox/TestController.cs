using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;

namespace SocketTry.Sandbox
{
    [Route("/api")]
    [Controller]
    public class TestController : BaseController
    {

        [HttpGet]
        public Card GetTest()
        {
            var a = new Card
            {
                Name = "Fire Goblin",
                Damage = 34
            };
            return a;
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
