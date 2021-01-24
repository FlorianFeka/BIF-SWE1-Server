using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Models.Types;
using MonsterTradingCardsGame.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MonsterTradingCardsGame.Repository
{
    public class DeckRepository
    {
        private readonly DatabaseConnection _connection;
        private readonly CardRepository _cardRepository;

        public DeckRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
            _cardRepository = SingletonFactory.GetObject<CardRepository>();
        }

        public Deck CreateDeck(Guid userId, Guid[] cardIds)
        {
            if (cardIds.Length != Deck.DeckSize) throw new Exception($"Number of cards not equivalent to deck size ({Deck.DeckSize})");
            var deck = new Deck();
            using var sqlTransaction = _connection.GetConnection().BeginTransaction();
            SqlCommand createDeckCommand = new SqlCommand(_createDeckCommandString, _connection.GetConnection());
            createDeckCommand.Transaction = sqlTransaction;

            createDeckCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16));
            createDeckCommand.Parameters.Add(new SqlParameter("@CardId", SqlDbType.UniqueIdentifier, 16));

            try
            {
                for (int i = 0; i < cardIds.Length; i++)
                {
                    createDeckCommand.Parameters[0].Value = userId;
                    createDeckCommand.Parameters[1].Value = cardIds[i];
                    if (createDeckCommand.ExecuteNonQuery() != 1) throw new Exception($"Problem adding card {cardIds[i]} to deck!");
                    deck.Cards[i] = cardIds[i];
                }
                sqlTransaction.Commit();
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                throw e;
            }
            return deck;
        }

        public Card[] GetDeck(Guid userId)
        {
            var cards = new List<Card>();
            SqlCommand command = new SqlCommand(_selectDeckWithUserIdCommandString, _connection.GetConnection());
            // select cards, parse them and return
            command.Parameters.Add(Utils.CreateSqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16, userId));
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var monsterString = reader.GetString(3);
                MonsterType? monster = null;
                if(!string.IsNullOrEmpty(monsterString))
                {
                    monster = Enum.Parse<MonsterType>(monsterString, true);
                }
                var card = new Card
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Damage = reader.GetDouble(2),
                    Monster = monster,
                    Element = Enum.Parse<ElementType>(reader.GetString(4), true),
                    IsSpell = reader.GetBoolean(5)
                };
            }
            return cards.ToArray();
        }

        private readonly string _createDeckCommandString = "INSERT INTO [dbo].[Deck] ([UserId],[CardId])" +
            "VALUES (@UserId,@CardId)";

        private readonly string _selectDeckWithUserIdCommandString = "SELECT [Id],[Name],[Damage],[Monster],[Element],[IsSpell],[UserId]" +
            "FROM [MonsterTradingCardGame].[dbo].[Cards]" +
            "WHERE [Id] IN (SELECT [CardId]" +
            "FROM [MonsterTradingCardGame].[dbo].[Deck]" +
            "WHERE [UserId] = @UserId)";
    }
}
