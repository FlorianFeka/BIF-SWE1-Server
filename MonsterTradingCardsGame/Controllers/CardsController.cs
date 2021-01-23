using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/cards")]
    [Controller]
    public class CardsController : BaseController
    {
        public SessionService _sessionService;
        private readonly CardService _cardService;

        public CardsController()
        {
            _sessionService = SingletonFactory.GetObject<SessionService>();
            _cardService = SingletonFactory.GetObject<CardService>();
        }

        [HttpGet]
        public IEnumerable<Card> GetCards()
        {
            var session = Utils.GetSession(_sessionService, HttpRequest, HttpResponse);
            if (session == null)
            {
                return null;
            }
            return _cardService.GetCards(session.UserId);
        }
    }
}
