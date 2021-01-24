using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SimpleServer;
using SimpleServer.Attributes;
using SimpleServer.Attributes.Verbs;
using SimpleServer.Http;
using System;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/transactions/packages")]
    [Controller]
    public class TransactionsController : BaseController
    {
        private readonly SessionService _sessionService;
        private readonly TransactionService _transactionService;
        
        public TransactionsController()
        {
            _sessionService = SingletonFactory.GetObject<SessionService>();
            _transactionService = SingletonFactory.GetObject<TransactionService>();
        }

        [HttpPost]
        public string BuyPackage()
        {
            var session = Utils.GetSession(_sessionService, HttpRequest, HttpResponse);
            if (session == null)
            {
                return null;
            }
            try
            {
                _transactionService.BuyPackage(session.UserId);
            }
            catch (Exception e)
            {
                HttpResponse.SetStatus(HttpStatus.Bad_Request);
                return e.Message;
            }
            
            return "Package bought!";
        }
    }
}
