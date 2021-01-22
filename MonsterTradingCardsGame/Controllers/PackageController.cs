using MonsterTradingCardsGame.Models.Cards;
using MonsterTradingCardsGame.Models.Packages;
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
        public Package CreatePackage(CardDto[] cardsDto)
        {
            if(Utils.GetSession(_sessionService, HttpRequest, HttpResponse) == null)
            {
                return null;
            }
            var cards = cardsDto.Select(x =>
            {
                var card = new Card
                {
                    Id = x.Id,
                    Name = x.Name,
                    Damage = x.Damage,
                };
                card.SetTypes();
                return card;
            });
            if (cardsDto == null || cardsDto.Length != 5)
            {
                HttpResponse.SetContent("No cards or not the right amount of cards!");
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return null;
            }
            return _packageService.CreatePackage(cards.ToArray());
        }
    }
}
