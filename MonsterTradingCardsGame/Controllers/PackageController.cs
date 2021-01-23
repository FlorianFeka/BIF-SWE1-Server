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
    public class PackageController : BaseController
    {
        private SessionService _sessionService;
        private PackageService _packageService;

        public PackageController()
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
            if (cards == null || cards.Length != 5)
            {
                HttpResponse.SetContent("No cards or not the right amount of cards!");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            return _packageService.CreatePackage(cards.ToArray());
        }
    }
}
