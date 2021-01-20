using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;

namespace SocketTry.Sandbox
{
    // TODO: Check for cases like this where one controller is part of the other for example in
    // TestController.cs route: /api and if here /api/test should throw error
    [Route("/rapi/test")]
    [Controller]
    public class OtherTestController
    {

        [HttpGet]
        public string GetTest()
        {
            return "[OtherTestController]: Empty Test Route";
        }

        [HttpGet("{id}")]
        public string GetTestWith(string id, string body)
        {
            return $"[OtherTestController]: ID: {id}\nBody: {body}";
        }
    }
}
