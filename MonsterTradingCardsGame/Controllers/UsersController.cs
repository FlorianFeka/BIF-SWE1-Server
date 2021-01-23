using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;
using SocketTry.Http;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/users")]
    [Controller]
    public class UsersController : BaseController
    {
        private UserService _userService;

        public UsersController()
        {
            _userService = SingletonFactory.GetObject<UserService>();
        }

        [HttpPost]
        public User CreateUser(User user)
        {
            if(user == null || user.Username == null || user.Password == null)
            {
                HttpResponse.SetContent("Invalid User! (╯ ͠° ͟ʖ ͡°)╯┻━┻");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            if (_userService.UserExists(user.Username))
            {
                HttpResponse.SetContent("User already exists! ¯\\_(ツ)_/¯");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            if (_userService.CreateUser(user))
            {
                HttpResponse.SetStatus(HttpStatus.Created);
            }
            return user;
        }
    }
}
