using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Repository
{
    public class PackageRepository
    {
        private readonly DatabaseConnection _connection;
        
        public PackageRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
        }

        public Package CreatePackage(Card[] cards)
        {
            var package = new Package();
            using var sqlTransaction = _connection.GetConnection().BeginTransaction();
            SqlCommand createCardCommand = new SqlCommand(_createCardCommandString, _connection.GetConnection());
            SqlCommand createPackageCommand = new SqlCommand(_createPackageCommandString, _connection.GetConnection());
            createCardCommand.Transaction = sqlTransaction;
            createPackageCommand.Transaction = sqlTransaction;

            createCardCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier, 16));
            createCardCommand.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 255));
            createCardCommand.Parameters.Add(new SqlParameter("@Damage", SqlDbType.Float, 8));
            createCardCommand.Parameters.Add(new SqlParameter("@Monster", SqlDbType.VarChar, 255));
            createCardCommand.Parameters.Add(new SqlParameter("@Element", SqlDbType.VarChar, 255));
            createCardCommand.Parameters.Add(new SqlParameter("@IsSpell", SqlDbType.Bit, 1));

            createPackageCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier, 16));
            createPackageCommand.Parameters.Add(new SqlParameter("@CardId", SqlDbType.UniqueIdentifier, 16));

            try
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    var card = cards[i];
                    createCardCommand.Parameters[0].Value = card.Id;
                    createCardCommand.Parameters[1].Value = card.Name;
                    createCardCommand.Parameters[2].Value = card.Damage;
                    createCardCommand.Parameters[3].Value = card.Monster == null ? "" : card.Monster.ToString();
                    createCardCommand.Parameters[4].Value = card.Element;
                    createCardCommand.Parameters[5].Value = card.IsSpell;
                    if (createCardCommand.ExecuteNonQuery() != 1) throw new Exception($"Problem saving card {card.Id}!");

                    createPackageCommand.Parameters[0].Value = package.Id;
                    createPackageCommand.Parameters[1].Value = card.Id;
                    if (createPackageCommand.ExecuteNonQuery() != 1) throw new Exception($"Problem saving card {card.Id} into package!");
                    package.Cards[i] = card.Id;
                }
                sqlTransaction.Commit();
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                throw e;
            }
            return package;
        }

        private readonly string _createCardCommandString = "INSERT INTO [dbo].[Cards] ([Id],[Name],[Damage],[Monster],[Element],[IsSpell])" +
            "VALUES (@Id,@Name,@Damage,@Monster,@Element,@IsSpell)";
        private readonly string _createPackageCommandString = "INSERT INTO [dbo].[Packages] ([Id],[CardId])" +
            "VALUES (@Id,@CardId)";


    }
}
