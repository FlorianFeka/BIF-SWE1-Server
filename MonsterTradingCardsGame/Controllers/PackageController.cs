using MonsterTradingCardsGame.Models.Cards;
using MonsterTradingCardsGame.Models.Packages;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/packages")]
    [Controller]
    public class PackageController : BaseController
    {
        [HttpPost]
        public Package CreatePackage(CardDto[] cards)
        {
            var a = cards;
            return new Package();
        }
    }
}
