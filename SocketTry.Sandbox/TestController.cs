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
        public Card GetTestWith(int id, Card body)
        {
            id += 2000;
            body.Damage = id;
            return body;
        }

        [HttpPost]
        public void PostSomething()
        {
            HttpResponse.AddHeader("SomeRandomTestHeader", "someRandomValue");
            HttpResponse.StatusCode = 204;
            HttpResponse.Status = "No Content";
        }
    }
}
