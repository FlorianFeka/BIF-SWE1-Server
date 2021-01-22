using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Models.Sessions;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;
using SocketTry.Http;
using System;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/sessions")]
    [Controller]
    public class SessionsController : BaseController
    {
        private UserService _userService;
        private SessionService _sessionService;

        public SessionsController()
        {
            _userService = SingletonFactory.GetObject<UserService>();
            _sessionService = SingletonFactory.GetObject<SessionService>();
        }

        [HttpPost]
        public SessionDto CreateSession(User user)
        {
            if (user == null || user.Username == null || user.Password == null)
            {
                HttpResponse.SetContent("No valid user in message!");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            try
            {
                var fetchedUser = _userService.CheckPassword(user.Username, user.Password);
                if(fetchedUser == null)
                {
                    HttpResponse.SetContent("Invalid Credentials!");
                    HttpResponse.SetStatus(HttpStatus.Bad_Request);
                    return null;
                }
                var session = _sessionService.SaveSession(fetchedUser);
                return new SessionDto
                {
                    Token = session.Token,
                    ExpiryDate = session.ExpiryDate,
                };
            }
            catch(Exception e)
            {
                HttpResponse.SetContent($"{e.Message}\n{e.StackTrace}");
                HttpResponse.SetStatus(HttpStatus.Internal_Server_Error);
                return null;
            }
        }
    }
}
