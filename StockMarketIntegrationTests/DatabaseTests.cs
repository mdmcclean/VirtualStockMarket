using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockGameService.Models;

namespace Capstone
{

    /// <summary>
    /// Testing our DAL Methods
    /// Getting data from database, "NPGeek"
    /// </summary>
    [TestClass]
    public class DALIntegrationTests
    {
        private TransactionScope _tran;
        private string _connectionString = @"Data Source =.\SQLEXPRESS;Initial Catalog = StockGame; Integrated Security = True";

        //Added from the vending machine
        private int _userId1 = -1;
        private int _userId2 = -1;
        private int _gameId = 0;

        /*
        * SETUP.
        */
        [TestInitialize]
        public void Initialize()
        {
            _tran = new TransactionScope();
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            PasswordHelper passHelper = new PasswordHelper("Abcd!234");
            if (_userId1 == -1)
            {
                var temp = new UserItem() { Id = -1 };
                temp.FirstName = "Amy";
                temp.LastName = "Rupp";
                temp.Username = "anr";
                temp.Hash = passHelper.Hash;
                temp.Salt = passHelper.Salt;
                temp.Email = "amy@tech.com";
                temp.RoleId = (int)RoleManager.eRole.Player;

                // Add user item
                _userId1 = _dal.AddUserItem(temp);
                Assert.AreNotEqual(0, _userId1);
            }

            if (_userId2 == -1)
            {
                var temp = new UserItem() { Id = -1 };
                temp.FirstName = "Chloe";
                temp.LastName = "Rupp";
                temp.Username = "ccr";
                temp.Hash = passHelper.Hash;
                temp.Salt = passHelper.Salt;
                temp.Email = "chloe@tech.com";
                temp.RoleId = (int)RoleManager.eRole.Player;

                // Add user item
                _userId2 = _dal.AddUserItem(temp);
                Assert.AreNotEqual(0, _userId2);
            }
        }

        /*
        * CLEANUP.
        * Rollback the Transaction.  
        */
        [TestCleanup]
        public void Cleanup()
        {
            _tran.Dispose();
        }

        /*
        * TEST.
        */
        [TestMethod]
        public void AddUserGame()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            Game game = new Game()
            {
                TimeStarted = DateTime.Now,
                Duration = 10
            };

            int _gameId = _dal.NewGame(game);

            //int userId = 9;
            int userId = _userId2;
            //int gameId = 2;
            int gameId = _gameId;

            //Act
            bool test = _dal.AddUserGame(userId);

            //Assert
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void AddUserStock()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            //int userId = 9;
            int userId = _userId2;
            int stockId = 1;
            int shares = 1;

            //Act
            bool test = _dal.AddUserStock(userId, stockId, shares);

            //Assert
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void AvailableStocks()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);

            //Act
            List<Stock> test = _dal.AvailableStocks();

            //Assert
            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void NewGame()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            Game game = new Game()
            {
                TimeStarted = DateTime.Now,
                Duration = 10
            };

            //Act
            int test = _dal.NewGame(game);

            //Assert
            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void SellStock()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            //int userId = 1;
            int userId = _userId2;
            int stockId = 1;
            int shares = 1;

            //Act
            bool test = _dal.SellStock(userId, stockId, shares);

            //Assert
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void UpdateStocks()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);

            //Act
            bool test = _dal.UpdateStocks();

            //Assert
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void UsersPlaying()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            int gameId = 3;

            //Act
            List<UserItem> test = _dal.UsersPlaying(gameId);

            //Assert
            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void UserStocks()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            //int userId = 1;
            int userId = _userId2;

            //Act
            List<UserStockItem> test = _dal.UserStocks(userId);

            //Assert
            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void WipeUserGame()
        {
            //Lucas - works if data is in database

            
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            int userId = _userId2;
            Game game = new Game()
            {
                TimeStarted = DateTime.Now,
                Duration = 10
            };
            int _gameId = _dal.NewGame(game);
            _dal.AddUserGame(userId);
            //int gameId = 2;

            //Act
            bool test = _dal.WipeUserGame(_gameId);

            //Assert
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WipeUserStock()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            int userId = _userId2;
            int stockId = 1;
            int shares = 1;
            _dal.AddUserStock(userId, stockId, shares);
            
            //Act
            bool test = _dal.WipeUserStock();

            //Assert
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void GetUserIdByUsername()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            string username = "ccr";

            //Act
            int test = _dal.GetUserIdByUsername(username);

            //Assert
            Assert.AreEqual(_userId2, test);
        }

        [TestMethod]
        public void GetCashAmounts()
        {
            //Arrange
            StockGameDAL _dal = new StockGameDAL(_connectionString);
            

            //Act
            List<UserCash> test = _dal.GetCashAmounts();

            //Assert
            Assert.IsNotNull(test);
        }
        /// <summary>
        /// Tests the user POCO methods from the vending machine
        /// </summary>
        [TestMethod()]
        public void TestUserMethods()
        {
            PasswordHelper passHelper = new PasswordHelper("Abcd!234");
            StockGameDAL _dal = new StockGameDAL(_connectionString);

            // Test add user
            UserItem item = new UserItem();
            item.FirstName = "Chris";
            item.LastName = "Rupp";
            item.Username = "cjr";
            item.Hash = passHelper.Hash;
            item.Salt = passHelper.Salt;
            item.Email = "chris@tech.com";
            item.RoleId = (int)RoleManager.eRole.Player;
            int id = _dal.AddUserItem(item);
            Assert.AreNotEqual(0, id);

            UserItem itemGet = _dal.GetUserItem(id);
            Assert.AreEqual(item.Id, itemGet.Id);
            Assert.AreEqual(item.FirstName, itemGet.FirstName);
            Assert.AreEqual(item.LastName, itemGet.LastName);
            Assert.AreEqual(item.Username, itemGet.Username);
            Assert.AreEqual(item.Hash, itemGet.Hash);
            Assert.AreEqual(item.Salt, itemGet.Salt);
            Assert.AreEqual(item.Email, itemGet.Email);

            // Test update user
            item.FirstName = "What";
            item.LastName = "What";
            item.Username = "What";
            item.Email = "What";
            item.Hash = "What";
            item.Salt = "What";
            Assert.IsTrue(_dal.UpdateUserItem(item));

            itemGet = _dal.GetUserItem(id);
            Assert.AreEqual(item.Id, itemGet.Id);
            Assert.AreEqual(item.FirstName, itemGet.FirstName);
            Assert.AreEqual(item.LastName, itemGet.LastName);
            Assert.AreEqual(item.Username, itemGet.Username);
            Assert.AreEqual(item.Hash, itemGet.Hash);
            Assert.AreEqual(item.Salt, itemGet.Salt);
            Assert.AreEqual(item.Email, itemGet.Email);

            // Test delete user
            _dal.DeleteUserItem(id);
            var users = _dal.GetUserItems();
            foreach (var user in users)
            {
                Assert.AreNotEqual(id, user.Id);
            }
        }
    }

}
