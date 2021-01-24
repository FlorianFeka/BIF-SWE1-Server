using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SimpleServer;
using SimpleServer.Attributes;
using SimpleServer.Attributes.Verbs;
using SimpleServer.Http;
using System;
using System.Linq;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/deck")]
    [Controller]
    public class DeckController : BaseController
    {
        private readonly SessionService _sessionService;
        private readonly DeckService _deckService;

        public DeckController()
        {
            _sessionService = SingletonFactory.GetObject<SessionService>();
            _deckService = SingletonFactory.GetObject<DeckService>();
        }

        [HttpGet]
        public Card[] GetDeck()
        {
            var session = Utils.GetSession(_sessionService, HttpRequest, HttpResponse);
            if (session == null)
            {
                return null;
            }
            var cards = _deckService.GetDeck(session.UserId);
            if(HttpRequest.Url.Parameter.TryGetValue("format", out var format))
            {
                if(format.Equals("plain", StringComparison.OrdinalIgnoreCase))
                {
                    var plainCards = string.Join("\n", cards.Select(x => x.ToString()));
                    HttpResponse.SetContent(plainCards);
                    return null;
                }
            }
            return cards;
        }

        [HttpPut]
        public Deck UpdateDeck(Guid[] cardIds)
        {
            var session = Utils.GetSession(_sessionService, HttpRequest, HttpResponse);
            if (session == null)
            {
                return null;
            }

            if (cardIds == null || cardIds.Length != Deck.DeckSize)
            {
                HttpResponse.SetContent($"No cards or not the right amount of cards! The right amount: {Deck.DeckSize}.");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            return _deckService.CreateDeck(session.UserId, cardIds);
        }
    }
}
