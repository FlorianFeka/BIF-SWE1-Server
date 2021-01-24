using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;
using System;

namespace MonsterTradingCardsGame.Services
{
    public class DeckService
    {
        private DeckRepository _deckRepository;

        public DeckService()
        {
            _deckRepository = SingletonFactory.GetObject<DeckRepository>();
        }

        public Deck CreateDeck(Guid userId, Guid[] cards)
        {
            return _deckRepository.CreateDeck(userId, cards);
        }

        public Card[] GetDeck(Guid userId)
        {
            return _deckRepository.GetDeck(userId);
        }
    }
}
