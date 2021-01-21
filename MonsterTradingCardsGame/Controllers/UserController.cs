using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;
using SocketTry.Http;
using System;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/users")]
    [Controller]
    public class UserController : BaseController
    {
        private UserService _userService;

        public UserController()
        {
            _userService = SingletonFactory.GetObject<UserService>();
        }

        [HttpPost]
        public User CreateUser(User user)
        {
            if (_userService.UserExists(user.Username))
            {
                HttpResponse.SetContent("User already exists!");
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
