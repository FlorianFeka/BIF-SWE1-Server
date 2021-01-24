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
        private readonly SessionService _sessionService;
        private readonly UserService _userService;

        public UsersController()
        {
            _sessionService = SingletonFactory.GetObject<SessionService>();
            _userService = SingletonFactory.GetObject<UserService>();
        }

        [HttpGet("{username}")]
        public User GetUser(string username)
        {
            var session = Utils.GetSession(_sessionService, HttpRequest, HttpResponse);
            if (session == null)
            {
                return null;
            }

            var userFromUsername = _userService.GetUserWithUsername(username);
            var userFromSession = _userService.GetUserWithId(session.UserId);

            if(userFromUsername == null)
            {
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                HttpResponse.SetContent("User requested not existing!");
                return null;
            }

            if(userFromUsername.Username != userFromSession.Username)
            {
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                HttpResponse.SetContent("Unauthorized for user requested!");
                return null;
            }

            return userFromUsername;
        }

        [HttpPut("{username}")]
        public User UpdateUser(string username, User user)
        {
            var session = Utils.GetSession(_sessionService, HttpRequest, HttpResponse);
            if (session == null)
            {
                return null;
            }

            var userFromUsername = _userService.GetUserWithUsername(username);
            var userFromSession = _userService.GetUserWithId(session.UserId);

            if (userFromUsername == null)
            {
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                HttpResponse.SetContent("User requested not existing!");
                return null;
            }

            if (userFromUsername.Username != userFromSession.Username)
            {
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                HttpResponse.SetContent("Unauthorized for user requested!");
                return null;
            }
            user.Id = userFromSession.Id;
            var updatedUser = _userService.UpdateUser(user);
            if(updatedUser == null)
            {
                HttpResponse.SetStatus(HttpStatus.Internal_Server_Error);
                HttpResponse.SetContent("Could not update user!");
                return null;
            }

            return updatedUser;
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
