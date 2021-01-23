using MonsterTradingCardsGame.Repository;
using MonsterTradingCardsGame.Util;
using System;

namespace MonsterTradingCardsGame.Services
{
    public class TransactionService
    {
        private readonly TransactionRepository _transactionRepository;

        public TransactionService()
        {
            _transactionRepository = SingletonFactory.GetObject<TransactionRepository>();
        }

        public void BuyPackage(Guid userId)
        {
            _transactionRepository.BuyPackage(userId);
        }
    }
}
