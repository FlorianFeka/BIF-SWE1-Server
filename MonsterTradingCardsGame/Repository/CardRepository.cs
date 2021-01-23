using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Repository
{
    public class CardRepository
    {
        private readonly DatabaseConnection _connection;

        public CardRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
        }

        public IEnumerable<Card> GetCards(Guid userId)
        {
            var cards = new List<Card>();
            SqlCommand command = new SqlCommand(_getCardsFromUserCommandString, _connection.GetConnection());
            command.Parameters.Add(Utils.CreateSqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16, userId));
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var card = new Card
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Damage = reader.GetDouble(2),
                };
                cards.Add(card);
            }
            return cards;
        }

        private readonly string _getCardsFromUserCommandString = "SELECT [Id],[Name],[Damage]" +
            "FROM [MonsterTradingCardGame].[dbo].[Cards] WHERE [UserId] = @UserId;";
    }
}
