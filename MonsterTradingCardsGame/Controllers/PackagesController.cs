using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SimpleServer;
using SimpleServer.Attributes;
using SimpleServer.Attributes.Verbs;
using SimpleServer.Http;
using System.Linq;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/packages")]
    [Controller]
    public class PackagesController : BaseController
    {
        private readonly SessionService _sessionService;
        private readonly PackageService _packageService;
        private readonly CardService _cardService;

        public PackagesController()
        {
            _sessionService = SingletonFactory.GetObject<SessionService>();
            _packageService = SingletonFactory.GetObject<PackageService>();
            _cardService = SingletonFactory.GetObject<CardService>();
        }

        [HttpPost]
        public Package CreatePackage(Card[] cards)
        {
            if(Utils.GetSession(_sessionService, HttpRequest, HttpResponse) == null)
            {
                return null;
            }
            cards.ToList().ForEach(x => x.SetTypes());
            if (cards == null || cards.Length != Package.PackageSize)
            {
                HttpResponse.SetContent($"No cards or not the right amount of cards! The right amount: {Package.PackageSize}.");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            foreach (var card in cards)
            {
                if (_cardService.CardExists(card.Id))
                {
                    HttpResponse.SetContent($"Card with Id: {card.Id} already exists");
                    HttpResponse.SetStatus(HttpStatus.Bad_Request);
                    return null;
                }
            }
            return _packageService.CreatePackage(cards.ToArray());
        }
    }
}
