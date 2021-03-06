﻿using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;
using System;
using System.Collections.Generic;

namespace MonsterTradingCardsGame.Services
{
    public class CardService
    {
        public readonly CardRepository _cardRepository;

        public CardService()
        {
            _cardRepository = SingletonFactory.GetObject<CardRepository>();
        }

        public IEnumerable<Card> GetCards(Guid userId)
        {
            return _cardRepository.GetCards(userId);
        }

        public bool CardExists(Guid cardId)
        {
            return _cardRepository.CardExists(cardId);
        }
    }
}
