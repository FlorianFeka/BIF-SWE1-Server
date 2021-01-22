﻿using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Models.Sessions;
using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;

namespace MonsterTradingCardsGame.Services
{
    public class SessionService
    {
        private SessionRepository _sessionRepository;

        public SessionService()
        {
            _sessionRepository = SingletonFactory.GetObject<SessionRepository>();
        }

        public Session SaveSession(User user)
        {
            return _sessionRepository.SaveSession(user);
        }

        public bool ValidSession(string token)
        {
            return _sessionRepository.ValidSession(token);
        }
    }
}
