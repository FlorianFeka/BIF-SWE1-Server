using MonsterTradingCardsGame.Models;
using MonsterTradingCardsGame.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MonsterTradingCardsGame.Repository
{
    public class TransactionRepository
    {
        private readonly DatabaseConnection _connection;
        private readonly UserRepository _userRepository;

        public TransactionRepository()
        {
            _connection = SingletonFactory.GetObject<DatabaseConnection>();
            _userRepository = SingletonFactory.GetObject<UserRepository>();
        }

        public Guid? GetRandomPackage()
        {
            SqlCommand getRandomPackageCommand = new SqlCommand(_getRandomPackageCommandString, _connection.GetConnection());
            using var reader = getRandomPackageCommand.ExecuteReader();
            if (reader.Read())
            {
                var packageId = reader.GetGuid(0);
                return packageId;
            }
            return null;
        }

        public void BuyPackage(Guid userId)
        {
            var packageId = GetRandomPackage();
            if (packageId == null) throw new Exception("No packages available!");
            var user = _userRepository.GetUserWithId(userId);
            if (user == null) throw new Exception("User not found!");
            if (user.Money < Package.PackageCost) throw new Exception("User has not enough money to buy a package!");

            SqlCommand updateCardOwnershipCommand = new SqlCommand(_updateCardOwnershipCommandString, _connection.GetConnection());
            SqlCommand updateUsersMoneyCommand = new SqlCommand(_updateUsersMoneyCommandString, _connection.GetConnection());
            SqlCommand deletePackageWithIdCommand = new SqlCommand(_deletePackageWithIdCommandString, _connection.GetConnection());
            using var sqlTransaction = _connection.GetConnection().BeginTransaction();
            updateCardOwnershipCommand.Transaction = sqlTransaction;
            updateUsersMoneyCommand.Transaction = sqlTransaction;
            deletePackageWithIdCommand.Transaction = sqlTransaction;

            updateCardOwnershipCommand.Parameters.Add(Utils.CreateSqlParameter("@PackageId", SqlDbType.UniqueIdentifier, 16, packageId));
            updateCardOwnershipCommand.Parameters.Add(Utils.CreateSqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16, userId));
            updateUsersMoneyCommand.Parameters.Add(Utils.CreateSqlParameter("@UserId", SqlDbType.UniqueIdentifier, 16, userId));
            updateUsersMoneyCommand.Parameters.Add(Utils.CreateSqlParameter("@Money", SqlDbType.Float, 16, Convert.ToDouble(user.Money - Package.PackageCost)));
            deletePackageWithIdCommand.Parameters.Add(Utils.CreateSqlParameter("@Id", SqlDbType.UniqueIdentifier, 16, packageId));
            // execute and check if 5 rows were affected, if not rollback
            try
            {
                if (updateCardOwnershipCommand.ExecuteNonQuery() != Package.PackageSize)
                {
                    throw new Exception("Error when transfering ownership of the cards to user!");
                }
                if (updateUsersMoneyCommand.ExecuteNonQuery() != 1)
                {
                    throw new Exception("Problem with payment!");
                }
                if (deletePackageWithIdCommand.ExecuteNonQuery() != Package.PackageSize)
                {
                    throw new Exception("Problem with deleting the package!");
                }
                sqlTransaction.Commit();
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                throw e;
            }
        }

        private readonly string _getRandomPackageCommandString = "SELECT TOP (1) [Id]" +
            "FROM [MonsterTradingCardGame].[dbo].[Packages] ORDER BY NEWID();";
        private readonly string _updateCardOwnershipCommandString = "UPDATE [MonsterTradingCardGame].[dbo].[Cards]" +
            "SET [UserId] = @UserId Where [Id] IN" +
            "(SELECT [CardId] FROM [MonsterTradingCardGame].[dbo].[Packages]" +
            "WHERE [Id] = @PackageId);";
        private readonly string _deletePackageWithIdCommandString = "DELETE FROM [dbo].[Packages] WHERE [Id] = @Id";
        private readonly string _updateUsersMoneyCommandString = "UPDATE [MonsterTradingCardGame].[dbo].[Users]" +
            "SET [Money] = @Money WHERE [Id] = @UserId";
    }
}
