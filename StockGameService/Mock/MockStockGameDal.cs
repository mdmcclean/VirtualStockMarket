using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Capstone;
using StockGameService.Models;

namespace StockGameService.Mock
{
    public class MockStockGameDal : IStockGameDAL
    {
        public List<Stock> _stocks = new List<Stock>();
        public List<UserStockItem> userStocks = new List<UserStockItem>();
        public List<UserItem> users = new List<UserItem>();
        public MockStockGameDal()
        {
        }
        public int AddRoleItem(RoleItem item)
        {
            throw new NotImplementedException();
        }

        public bool AddUserGame(int userId)
        {
            throw new NotImplementedException();
        }

        public int AddUserItem(UserItem item)
        {
            throw new NotImplementedException();
        }

        public bool AddUserStock(int userId, int stockId, int shares)
        {
            bool isAlreadyPurchased = false;
            foreach(UserStockItem stock in userStocks)
            {
                if(stock.UserStock.StockID == stockId)
                {
                    stock.Shares += shares;
                    isAlreadyPurchased = true;
                }
            }
            Stock currentStock = new Stock();
            foreach(Stock stock in _stocks)
            {
                if(stock.StockID == stockId)
                {
                    currentStock = stock;
                }
            }

            if(!isAlreadyPurchased)
            {
                userStocks.Add(new UserStockItem
                {
                    Shares = shares,
                    UserStock = currentStock
                });
            }
            return true;
        }

        public List<Stock> AvailableStocks()
        {
            bool hi = UpdateStocks();
            Stock stock1 = new Stock()
            {
                CompanyName = "Stock One",
                CurrentPrice = 45.69,
                StockID = 1,
                Symbol = "S1"
            };
            Stock stock2 = new Stock()
            {
                CompanyName = "Stock Two",
                CurrentPrice = 45.32,
                StockID = 2,
                Symbol = "STWO"
            };

            _stocks.Add(stock1);
            _stocks.Add(stock2);
            return _stocks;
        }

        public void DeleteUserItem(int userId)
        {
            throw new NotImplementedException();
        }

        public RoleItem GetRoleItemFromReader(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        public List<RoleItem> GetRoleItems()
        {
            throw new NotImplementedException();
        }

        public List<UserCash> GetCashAmounts()
        {
            throw new NotImplementedException();
        }

        public int GetUserIdByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public UserItem GetUserItem(int userId)
        {
            throw new NotImplementedException();
        }

        public UserItem GetUserItem(string username)
        {
            throw new NotImplementedException();
        }

        public UserItem GetUserItemFromReader(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        public List<UserItem> GetUserItems()
        {
            throw new NotImplementedException();
        }

        public int NewGame(Game gameModel)
        {
            throw new NotImplementedException();
        }

        public bool SellStock(int userId, int stockId, int shares)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStocks()
        {
            foreach(Stock stock in _stocks)
            {
                stock.CurrentPrice += 1;
            }
            return true;
        }

        public bool UpdateUserItem(UserItem item)
        {
            throw new NotImplementedException();
        }

        public List<UserItem> UsersPlaying(int gameId)
        {
            UserItem user1 = new UserItem()
            {
                FirstName = "Lucas",
                LastName = "F",
                Email = "a@aol.com",
                Hash = "hash",
                Salt = "salt",
                Id = 1,
                isReady = true,
                RoleId = 0,
                Username = "bbm9"
            };
            UserItem user2 = new UserItem()
            {
                FirstName = "M",
                LastName = "mcclean",
                Email = "d@aol.com",
                Hash = "hash",
                Salt = "salt",
                Id = 2,
                isReady = true,
                RoleId = 0,
                Username = "bbm9"
            };
            users.Add(user1);
            users.Add(user2);
            return users;
        }

        public List<UserStockItem> UserStocks(int id)
        {
            return userStocks;
        }

        public bool WipeUserGame(int gameId)
        {
            throw new NotImplementedException();
        }

        public bool WipeUserStock()
        {
            throw new NotImplementedException();
        }

        public List<OwnerOfStock> GetOwners()
        {
            throw new NotImplementedException();
        }

        public DateTime TimeEnd()
        {
            throw new NotImplementedException();
        }

        public Game SetGame(Settings these)
        {
            throw new NotImplementedException();
        }

        public bool Setup(Settings model)
        {
            throw new NotImplementedException();
        }

        public bool CheckSetting()
        {
            throw new NotImplementedException();
        }

        public bool SwitchSettings(bool isSetting)
        {
            throw new NotImplementedException();
        }

        string IStockGameDAL.CheckSetting()
        {
            throw new NotImplementedException();
        }

        public bool SwitchSettings(int settingNumber)
        {
            throw new NotImplementedException();
        }

        public bool EndGame()
        {
            throw new NotImplementedException();
        }
    }
}
