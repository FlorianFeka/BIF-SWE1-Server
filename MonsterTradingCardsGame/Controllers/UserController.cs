using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/users")]
    [Controller]
    public class UserController : BaseController
    {
        [HttpPost]
        public string CreateUser()
        {
            return "";
        }
    }
}
