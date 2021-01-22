using MonsterTradingCardsGame.Models.Cards;
using MonsterTradingCardsGame.Models.Packages;
using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;

namespace MonsterTradingCardsGame.Services
{
    public class PackageService
    {
        private PackageRepository _packageRepository;

        public PackageService()
        {
            _packageRepository = SingletonFactory.GetObject<PackageRepository>();
        }

        public Package CreatePackage(Card[] cards)
        {
            return _packageRepository.CreatePackage(cards);
        }
    }
}
