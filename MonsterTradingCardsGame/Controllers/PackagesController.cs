using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;
using SocketTry.Http;
using System.Linq;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/packages")]
    [Controller]
    public class PackagesController : BaseController
    {
        private readonly SessionService _sessionService;
        private readonly PackageService _packageService;

        public PackagesController()
        {
            _sessionService = SingletonFactory.GetObject<SessionService>();
            _packageService = SingletonFactory.GetObject<PackageService>();
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
            return _packageService.CreatePackage(cards.ToArray());
        }
    }
}
