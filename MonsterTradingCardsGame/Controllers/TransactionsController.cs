using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Services;
using MonsterTradingCardsGame.Util;
using SocketTry;
using SocketTry.Attributes;
using SocketTry.Attributes.Verbs;
using SocketTry.Http;
using System;

namespace MonsterTradingCardsGame.Controllers
{
    [Route("/transactions/packages")]
    [Controller]
    public class TransactionsController : BaseController
    {
        public SessionService _sessionService;
        public TransactionService _transactionService;
        
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
            
            HttpResponse.SetStatus(HttpStatus.OK);
            return "Package bought!";
        }
    }
}
