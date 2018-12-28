using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Capstone;
using StockGameService.Models;
using System.Diagnostics;
using System.Threading;

namespace Capstone
{
    public class StockGameDAL: IStockGameDAL
    {
        private string _connectionString;
        private const string _getLastIdSQL = "SELECT CAST(SCOPE_IDENTITY() as int);";

        public StockGameDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool AddUserGame(int userId)
        {
            //move this to wherever we want the game set up;
            //change below user to your login
            if(userId == 1)
            {
                //int game = 0;
                //game = CurrentGame();
                //Game ugh = Michaelsetsgame(game);
                //ugh.TimeStarted = DateTime.Now.AddSeconds(ugh.Duration);
                //set timer

                //call settings
            }
            //
            bool result = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "SELECT * FROM [User_Game] WHERE UserId = @userId AND GameId = (SELECT TOP(1) GameId FROM Game ORDER BY GameId DESC)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userid", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = true;
                    return result;
                }
            }
            string query = @"INSERT [User_Game] (UserId, GameId, CurrentCash, Total, IsReady) VALUES (@userid, (SELECT TOP(1) GameId FROM Game ORDER BY GameId DESC), @currentcash, @total,  0)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@currentcash", 100000);
                cmd.Parameters.AddWithValue("@total", 100000);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        public string CheckSetting()
        {
            string query = "Select [Settings].[Value] From [Settings] Where [Key] = 'SettingsGuy'";
            string setting = "";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                     setting = Convert.ToString(reader["Value"]);
                }
            }
            return setting;

        }
        public bool SwitchSettings(int settingNumber)
        {
            string query = "";
            bool isSetting = false;

            if(settingNumber == 1)
            {
                 query = @"Update [Settings] Set [Value] = 1 Where [Key] = 'SettingsGuy'";
            }
            else if (settingNumber == 2)
            {
                query = @"Update [Settings] Set [Value] = 2 Where [Key] = 'SettingsGuy'";
            }
            else
            {
                query = @"Update [Settings] Set [Value] = 0 Where [Key] = 'SettingsGuy'";
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected > 0)
                {
                    isSetting = true;
                }
            }
            return isSetting;
        }


        public bool AddUserStock(int userId, int stockId, int shares)
        {
            double userCash = 0;

            string userCashQuery = @"Select CurrentCash from [User_Game] Where [User_Game].UserId = @userId; Update [User_Game] Set IsReady = 1 where UserId = @userId;";

            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(userCashQuery, conn);

                cmd.Parameters.AddWithValue("@userId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userCash = Convert.ToDouble(reader["CurrentCash"]);
                }
            }

            double amountOfTrade = 0;
            int sharesAvailable = 0;
            string pricePerShareQuery = @"Select CurrentPrice, AvailableShares from [Stock] Where [Stock].StockId = @stockId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(pricePerShareQuery, conn);

                cmd.Parameters.AddWithValue("@stockId", stockId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    amountOfTrade = Convert.ToDouble(reader["CurrentPrice"])*shares;
                    sharesAvailable = Convert.ToInt32(reader["AvailableShares"]);
                }
            }

            bool result = false;

            if (userCash >= amountOfTrade && sharesAvailable >= shares)
            {
                string checkQuery = @"Update [User_Stocks] Set NumberOfShares = (NumberOfShares + @shares), PurchasePrice = " +
                                            "(((Select PurchasePrice from [User_Stocks] Where UserId = @userId AND StockId = @stockId) " +
                                            "* (Select NumberOfShares from [User_Stocks] Where UserId = @userId AND StockId = @stockId)) + " +
                                            "(@shares * (Select CurrentPrice from Stock Where StockId = @stockId)))/((Select NumberOfShares from " +
                                            "[User_Stocks] where UserId = @userId AND StockId = @stockId) + @shares) WHERE UserId = @userId AND StockId = @stockid; " +
                                            "Update [Stock] Set AvailableShares = AvailableShares - @shares WHERE [Stock].StockId = @stockId;";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(checkQuery, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@stockId", stockId);
                    cmd.Parameters.AddWithValue("@shares", shares);
                    int numberOfRowsAffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsAffected > 1)
                    {
                        result = true;
                    }
                }

                if (!result)
                {


                    string query = @"INSERT [User_Stocks] (UserId, StockId, PurchasePrice, NumberOfShares) VALUES (@userId, @stockId, (Select CurrentPrice from Stock Where StockId = @stockId) , @shares)";

                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@stockId", stockId);
                        cmd.Parameters.AddWithValue("@shares", shares);
                        int numberOfRowsAffected = cmd.ExecuteNonQuery();
                        if (numberOfRowsAffected > 0)
                        {
                            result = true;
                        }
                    }
                }

                string updateCash = @"Update [User_Game] Set CurrentCash = CurrentCash - @amountOfTrade, Total = CurrentCash - @amountOfTrade + (Select Sum([User_Stocks].NumberOfShares * [Stock].CurrentPrice) " +
                                                "From [User_Stocks] " +
                                                "Join [Stock] on [User_Stocks].StockId = [Stock].StockId " +
                                                "Where [User_Stocks].UserId = @userId) Where [User_Game].UserId = @userId";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(updateCash, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@amountOfTrade", amountOfTrade);
                    int numberOfRowsAffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }
           return result;
        }
        public List<Stock> AvailableStocks()
        {
            List<Stock> StockList = new List<Stock>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "select * from Stock";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Stock stockModel = new Stock();
                    stockModel.CompanyName = reader["CompanyName"].ToString();
                    stockModel.CurrentPrice = Convert.ToDouble(reader["CurrentPrice"]);
                    stockModel.AvailableShares = Convert.ToInt32(reader["AvailableShares"]);
                    //double.Parse(reader["CurrentPrice"].ToString())
                    stockModel.StockID = (int)reader["StockID"];
                    stockModel.Symbol = reader["Symbol"].ToString();

                    StockList.Add(stockModel);
                }
            }
            return StockList;
        }

        public int NewGame(Game gameModel)
        {
                gameModel.Duration = 600;
                gameModel.TimeStarted = DateTime.Now.AddSeconds(gameModel.Duration);
                

                string query = @"INSERT INTO [Game] (Duration, TimeStarted) VALUES (@duration, @timestarted)";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@duration", gameModel.Duration);
                    cmd.Parameters.AddWithValue("@timestarted", gameModel.TimeStarted);
                    int numberOfRowsAffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsAffected == 0)
                    {
                    throw new Exception();
                    }

                }


                string nextquery = @"Select GameId From [Game] where Duration = @duration AND TimeStarted = @timestarted";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(nextquery, conn);
                    cmd.Parameters.AddWithValue("@duration", gameModel.Duration);
                    cmd.Parameters.AddWithValue("@timestarted", gameModel.TimeStarted);
                    int GameID = (int)(cmd.ExecuteScalar());
                    if (GameID > 0)
                    {
                        return GameID;
                    }
                    else
                    {
                    throw new Exception();
                    }

                }
        }
        public List<OwnerOfStock> GetOwners()
        {
            List<OwnerOfStock> stockOwners = new List<OwnerOfStock>();
            string ownerQuery = "Select [User_Stocks].StockId, [User].Id, [User].FirstName, [User].LastName from[User_Stocks] Join[User] on[User_Stocks].UserId= [User].Id Where NumberOfShares > (Select [Settings].[Value]/2 From [Settings] Where [Settings].[Key] = 'AvailableStocks')";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                
                SqlCommand cmd = new SqlCommand(ownerQuery, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserItem userItem = new UserItem();
                    Stock userStock = new Stock();
                    OwnerOfStock item = new OwnerOfStock();
                    userItem.FirstName = Convert.ToString(reader["FirstName"]);
                    userItem.LastName = Convert.ToString(reader["LastName"]);
                    userItem.Id = Convert.ToInt32(reader["Id"]);
                    userStock.StockID = Convert.ToInt32(reader["StockId"]);
                    item.Owner = userItem;
                    item.StockOwned = userStock;
                    stockOwners.Add(item);

                }
            }
            return stockOwners;
        }
        public Game SetGame(Settings these)
        {
            
            Game gameModel = new Game();
            gameModel.GameID = CurrentGame();
            gameModel.Duration = (these.Timer * 60);
            gameModel.TimeStarted = DateTime.Now.AddSeconds(gameModel.Duration);
                
            Game gameboy = Michaelsetsgame(gameModel);
            return gameboy; 
            //set timer

                //call settings
            
        }
        public bool EndGame()
        {
            bool gameend = false;

            Game gameModel = new Game();
            gameModel.GameID = CurrentGame();
            gameModel.Duration = 1;
            gameModel.TimeStarted = DateTime.Now.AddSeconds(gameModel.Duration);
            Game gameboy = Michaelsetsgame(gameModel);

            return gameend;
        }
        public Game Michaelsetsgame(Game gameModel)
        {

            //Game gameModel = new Game();
            //gameModel.GameID = gameid;
            //gameModel.Duration = 30;
            //gameModel.TimeStarted = DateTime.Now.AddSeconds(gameModel.Duration);


            
            string query = @"UPDATE [Game] SET Duration = @duration, TimeStarted = @timestarted WHERE GameId = (SELECT TOP(1) GameId FROM Game ORDER BY GameId DESC)";


            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@duration", gameModel.Duration);
                cmd.Parameters.AddWithValue("@timestarted", gameModel.TimeStarted);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected == 0)
                {
                    throw new Exception();
                }

            }
            return gameModel;
        }
        public DateTime TimeEnd()
        {
            

            DateTime timeend = new DateTime();
            string nextquery = @"Select TimeStarted From [Game] WHERE GameId = (SELECT TOP(1) GameId FROM Game ORDER BY GameId DESC)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(nextquery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    timeend = Convert.ToDateTime(reader["TimeStarted"]);
                    
                }
              
            }
            return timeend; 
        }


        public int CurrentGame()
        {
            int gamer = 0;
            string nextquery = "SELECT TOP(1) GameId FROM Game ORDER BY GameId DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(nextquery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    gamer = Convert.ToInt32(reader["GameId"]);
                    if (gamer !=0)
                    {
                        return gamer;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }

            }
            return gamer;
        
        }
        public bool SellStock(int userId, int stockId, int shares)
        {

            bool result = false;
            int currentShares = 0;

            string checkIfOkay = @"Select NumberOfShares from [User_Stocks] Where UserId = @userId AND StockId = @stockId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                

                SqlCommand cmd = new SqlCommand(checkIfOkay, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@stockId", stockId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    currentShares = Convert.ToInt32(reader["NumberOfShares"]);
                }
            }

            if (currentShares > 0 && currentShares > shares)
            {
                string query = @"Update [User_Stocks] Set NumberOfShares = (NumberOfShares - @shares), PurchasePrice = " +
                                            "(((Select PurchasePrice from [User_Stocks] Where UserId = @userId AND StockId = @stockId) " +
                                            "* (Select NumberOfShares from [User_Stocks] Where UserId = @userId AND StockId = @stockId)) - " +
                                            "(@shares * (Select CurrentPrice from Stock Where StockId = @stockId)))/((Select NumberOfShares from " +
                                            "[User_Stocks] where UserId = @userId AND StockId = @stockId) - @shares) WHERE UserId = @userId AND StockId = @stockid; " +
                                            "Update [Stock] Set AvailableShares = AvailableShares + @shares WHERE [Stock].StockId = @stockId";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@stockId", stockId);
                    cmd.Parameters.AddWithValue("@shares", shares);
                    int numberOfRowsAffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            else if(currentShares == shares)
            {
                string query = @"Update [User_Stocks] Set NumberOfShares = (NumberOfShares - @shares), PurchasePrice = 0 WHERE UserId = @userId AND StockId = @stockid; " +
                                            "Update[Stock] Set AvailableShares = AvailableShares + @shares WHERE[Stock].StockId = @stockId;";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@stockId", stockId);
                    cmd.Parameters.AddWithValue("@shares", shares);
                    int numberOfRowsAffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsAffected > 0)
                    {
                        result = true;
                    }
                }

            }


            if (result)
            {
                string updateCash = @"Update [User_Game] Set CurrentCash = CurrentCash + @shares* (Select CurrentPrice from [Stock] Where [Stock].StockId = @stockId), Total = CurrentCash + @shares* (Select CurrentPrice from [Stock] Where [Stock].StockId = @stockId)+ (Select Sum([User_Stocks].NumberOfShares * [Stock].CurrentPrice) " +
                                                "From [User_Stocks] " +
                                                "Join [Stock] on [User_Stocks].StockId = [Stock].StockId " +
                                                "Where [User_Stocks].UserId = @userId) Where [User_Game].UserId = @userId";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(updateCash, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@shares", shares);
                    cmd.Parameters.AddWithValue("@stockId", stockId);
                    int numberOfRowsAffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsAffected > 0)
                    {
                        result = true;
                    }
                }

            }
            return result;

        }

        public bool UpdateStocks()
        {
            Random rnd = new Random();
            string query = "";
            int beginningUpdate = rnd.Next(25);
            double percentIncrease;
            for(int i = 1; i < 26; i++)
            {
                double increase = rnd.Next(10000);
                increase = increase - 5000;
                if(increase > 1000)
                {
                    if(increase > 1039 && increase < 1040)
                    {
                        percentIncrease = 1.25;
                    }
                    else if(increase > 1005 && increase < 1006)
                    {
                        percentIncrease = 0.75;
                    }
                    else
                    {
                        //increase = increase - 500;
                        percentIncrease = (((increase+200)/4) / 100000) + 1;
                    }
                }
                else
                {
                    percentIncrease = (((increase + 200) / 4) / 100000) + 1;
                }
                query += "Update [Stock] Set CurrentPrice = (CurrentPrice*" + percentIncrease + ") where StockId = " + beginningUpdate + "; ";
                beginningUpdate++;
                if(beginningUpdate > 25)
                {
                    beginningUpdate = beginningUpdate - 25;
                }
            }
            int numberOfRowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                numberOfRowsAffected = cmd.ExecuteNonQuery();

            }
            if(numberOfRowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public List<UserItem> UsersPlaying(int gameId)
        {
            List<UserItem> UserList = new List<UserItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "Select * From [User] " +
                                 "join [User_Game] on User_Game.UserId = [User].Id " +
                                 "where GameId = @gameid";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@gameid", gameId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserItem item = new UserItem();
                    item.Id = Convert.ToInt32(reader["Id"]);
                    item.FirstName = Convert.ToString(reader["FirstName"]);
                    item.LastName = Convert.ToString(reader["LastName"]);
                    item.Username = Convert.ToString(reader["Username"]);
                    item.Email = Convert.ToString(reader["Email"]);
                    item.Salt = Convert.ToString(reader["Salt"]);
                    item.Hash = Convert.ToString(reader["Hash"]);
                    item.RoleId = Convert.ToInt32(reader["RoleId"]);
                  
                    UserList.Add(item);
                }
            }
            return UserList; 
        }

        public List<UserStockItem> UserStocks(int userId)
        {
            List<UserStockItem> UserList = new List<UserStockItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "Select * from [User_Stocks] Join [Stock] on [Stock].StockId = [User_Stocks].StockId And [User_Stocks].UserId = @userId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserStockItem item = new UserStockItem();
                    Stock thisStock = new Stock();


                    thisStock.CompanyName = reader["CompanyName"].ToString();
                    thisStock.CurrentPrice = Convert.ToDouble(reader["CurrentPrice"]);
                    thisStock.StockID = (int)reader["StockID"];
                    thisStock.Symbol = reader["Symbol"].ToString();
                    item.UserStock = thisStock;
                    item.Shares = Convert.ToInt32(reader["NumberOfShares"]);
                    item.PurchasePrice = Convert.ToDouble(reader["PurchasePrice"]);
                    



                    UserList.Add(item);
                }
            }
            return UserList;
        }

        public bool WipeUserGame(int gameId)
        {
            bool result = false;

            string query = @"Delete From [User_Game] where GameId = @gameId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gameid", gameId);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        public bool WipeUserStock()
        {
            //Alex - all user stock? or pass id?
            //Lucas - works if there's something in table
            bool result = false;

            string checkQuery = @"DELETE From [User_Stocks]";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(checkQuery, conn);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        public int GetUserIdByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "Select Id From [User] where Username = @username";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);

                int UserId = (int)(cmd.ExecuteScalar());
                if(UserId > 0)
                {
                    return UserId;
                }
                else
                {
                    throw new Exception("didnt get user id by username");
                }
            }
        }

        public List<UserCash> GetCashAmounts()
        {
            List<UserCash> rtnList = new List<UserCash>();
            List<int> ids = new List<int>();
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string getIds = "Select [User_Game].UserId From [User_Game] Where IsReady = 1";
                SqlCommand cmd = new SqlCommand(getIds, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["UserId"]);
                    ids.Add(id);
                }
                
                
            }

            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                string updateStats = "";
                foreach(int userId in ids)
                {
                    updateStats += "Update[User_Game] Set Total = CurrentCash + (Select Sum([User_Stocks].NumberOfShares *[Stock].CurrentPrice) from[User_Stocks] " +
                        "Join [Stock] on Stock.StockId = [User_Stocks].StockId where[User_Stocks].UserId = " + userId + ") Where[User_Game].UserId = " + userId + ";";
                }
                conn.Open();

                SqlCommand cmd = new SqlCommand(updateStats, conn);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected > 0)
                {
                    bool didWork = true;
                }
             }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();


                string sql = "Select * from [User_Game] Join [User] on [User].Id = [User_Game].UserId ORDER By Total DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserCash userCash = new UserCash();
                    UserItem item = new UserItem();
                    userCash.CurrentCash = Convert.ToDouble(reader["CurrentCash"]);
                    userCash.TotalCash = Convert.ToDouble(reader["Total"]);
                    userCash.IdOfUser = Convert.ToInt32(reader["UserId"]);
                    item.FirstName = reader["FirstName"].ToString();
                    item.LastName = reader["LastName"].ToString();
                    item.Username = reader["Username"].ToString();
                    userCash.UserInfo = item;
                    rtnList.Add(userCash);
                }
            }


            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();


                string sql = @"Select [User].Id, Count([User].Id) As 'Owned' From [User] Join [User_Stocks] on [User_Stocks].UserId= [User].Id Where NumberOfShares > (Select [Settings].[Value] / 2 From [Settings] Where [Settings].[Key] = 'AvailableStocks') Group By [User].Id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int owned = Convert.ToInt32(reader["Owned"]);
                    int user = Convert.ToInt32(reader["Id"]);

                    foreach(UserCash person in rtnList)
                    {
                        if(person.IdOfUser == user)
                        {
                            person.OwnedStocks = owned;
                            break;
                        }
                    }
                }
            }


            return rtnList;
        }
        
       public bool Setup(Settings model)
        {
            int seconds = model.AvailableShares * 60;
            string timer = seconds.ToString();
            int availableshares = model.AvailableShares;
            bool result = false;
            string updateCash = @"Update [Settings] Set [Value] = @availableshares where [Key] = 'AvailableStocks'; " +
                "Update [Settings] Set [Value] = @timer where [Key] = 'Timer'; " +
                "Update [Stock] Set [Stock].AvailableShares = (Select [Settings].[Value] from [Settings] where [Settings].[Key] = 'AvailableStocks'); Update [User_Stocks] Set [User_Stocks].NumberOfShares = 0; Update[Stock] Set[Stock].CurrentPrice = 100; Update[User_Game] Set[User_Game].CurrentCash = 100000, Total = 100000;";
     

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(updateCash, conn);
                cmd.Parameters.AddWithValue("@availableshares", availableshares);
                cmd.Parameters.AddWithValue("@timer", timer);
                int numberOfRowsAffected = cmd.ExecuteNonQuery();
                if (numberOfRowsAffected > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        //public Settings whataresettings()
        //{
        //   Settings rules = new Settings();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();

        //        string sql = "select * from [Settings] where [Key] = 'Time' ";

        //        SqlCommand cmd = new SqlCommand(sql, conn);

        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            rules.Timer = Convert.ToInt32(reader["Value"]);
        //            stockModel.CurrentPrice = Convert.ToDouble(reader["CurrentPrice"]);
        //            stockModel.AvailableShares = (reader["AvailableShares"]);
        //            //double.Parse(reader["CurrentPrice"].ToString())
        //            stockModel.StockID = (int)reader["StockID"];
        //            stockModel.Symbol = reader["Symbol"].ToString();

        //            StockList.Add(stockModel);
        //        }
        //    }
        //    return StockList;
        //}

        //public int DisplayTimer(Game gameModel)
        //{

        //    var stopwatch = Stopwatch.StartNew();
        //    double secs = 0;
        //    //in seconds
        //    int gamelength = gameModel.Duration;

        //    while (secs < gamelength)
        //    {
        //        // Capture the elapsed ticks and write them to the console.
        //        secs = stopwatch.Elapsed.TotalSeconds;
        //        DateTime DisplayTimer = gameModel.TimeStarted - DateTime.Now;
        //        Console.WriteLine(secs);
        //    }

        //    // Capture the ticks again.
        //    // ... This will be a larger value.

        //    Console.WriteLine("Game Over you limey fucks");

        //    Console.ReadKey();
        //    //
        //    DateTime now = DateTime.Now;
        //    DateTime nowplus = now.AddMinutes(10);
        //    Console.WriteLine(now);
        //    Console.WriteLine(nowplus);
        //    Console.ReadKey();
        //}

        //public double GetTotalForUserGame(int id, int game)
        //{


        //public int GetUserbylowestId()
        //{
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();

        //        string sql = "Select Id From [User] where Username = @username";

        //        SqlCommand cmd = new SqlCommand(sql, conn);
        //        cmd.Parameters.AddWithValue("@username", username);

        //        int UserId = (int)(cmd.ExecuteScalar());
        //        if (UserId > 0)
        //        {
        //            return UserId;
        //        }
        //        else
        //        {
        //            throw new Exception("didnt get user id by username");
        //        }
        //    }
        //}

        #region UserItem Methods

        public int AddUserItem(UserItem item)
        {
            const string sql = "INSERT [User] (FirstName, LastName, Username, Email, Hash, Salt, RoleId) " +
                               "VALUES (@FirstName, @LastName, @Username, @Email, @Hash, @Salt, @RoleId);";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql + _getLastIdSQL, conn);
                cmd.Parameters.AddWithValue("@FirstName", item.FirstName);
                cmd.Parameters.AddWithValue("@LastName", item.LastName);
                cmd.Parameters.AddWithValue("@Username", item.Username);
                cmd.Parameters.AddWithValue("@Email", item.Email);
                cmd.Parameters.AddWithValue("@Hash", item.Hash);
                cmd.Parameters.AddWithValue("@Salt", item.Salt);
                cmd.Parameters.AddWithValue("@RoleId", item.RoleId);
                item.Id = (int)cmd.ExecuteScalar();
            }

            return item.Id;
        }

        public bool UpdateUserItem(UserItem item)
        {
            bool isSuccessful = false;

            const string sql = "UPDATE [User] SET FirstName = @FirstName, " +
                                                 "LastName = @LastName, " +
                                                 "Username = @Username, " +
                                                 "Email = @Email, " +
                                                 "Hash = @Hash, " +
                                                 "Salt = @Salt " +
                                                 "WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FirstName", item.FirstName);
                cmd.Parameters.AddWithValue("@LastName", item.LastName);
                cmd.Parameters.AddWithValue("@Username", item.Username);
                cmd.Parameters.AddWithValue("@Email", item.Email);
                cmd.Parameters.AddWithValue("@Hash", item.Hash);
                cmd.Parameters.AddWithValue("@Salt", item.Salt);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    isSuccessful = true;
                }
            }

            return isSuccessful;
        }

        public void DeleteUserItem(int userId)
        {
            const string sql = "DELETE FROM [User] WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public UserItem GetUserItem(int userId)
        {
            UserItem user = null;
            const string sql = "SELECT * From [User] WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    user = GetUserItemFromReader(reader);
                }
            }

            if (user == null)
            {
                throw new Exception("User does not exist.");
            }

            return user;
        }

        public List<UserItem> GetUserItems()
        {
            List<UserItem> users = new List<UserItem>();
            const string sql = "Select * From [User];";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(GetUserItemFromReader(reader));
                }
            }

            return users;
        }

        public UserItem GetUserItem(string username)
        {
            UserItem user = null;
            const string sql = "SELECT * From [User] WHERE Username = @Username;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    user = GetUserItemFromReader(reader);
                }
            }

            if (user == null)
            {
                throw new Exception("User does not exist.");
            }

            return user;
        }

        public UserItem GetUserItemFromReader(SqlDataReader reader)
        {
            UserItem item = new UserItem();

            item.Id = Convert.ToInt32(reader["Id"]);
            item.FirstName = Convert.ToString(reader["FirstName"]);
            item.LastName = Convert.ToString(reader["LastName"]);
            item.Username = Convert.ToString(reader["Username"]);
            item.Email = Convert.ToString(reader["Email"]);
            item.Salt = Convert.ToString(reader["Salt"]);
            item.Hash = Convert.ToString(reader["Hash"]);
            item.RoleId = Convert.ToInt32(reader["RoleId"]);

            return item;
        }

        #endregion

        #region RoleItem

        public int AddRoleItem(RoleItem item)
        {
            const string sql = "INSERT RoleItem (Id, Name) " +
                               "VALUES (@Id, @Name);";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql + _getLastIdSQL, conn);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@Name", item.Name);
                cmd.ExecuteNonQuery();
            }

            return item.Id;
        }

        public List<RoleItem> GetRoleItems()
        {
            List<RoleItem> roles = new List<RoleItem>();
            const string sql = "Select * From RoleItem;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    roles.Add(GetRoleItemFromReader(reader));
                }
            }

            return roles;
        }

        public RoleItem GetRoleItemFromReader(SqlDataReader reader)
        {
            RoleItem item = new RoleItem();

            item.Id = Convert.ToInt32(reader["Id"]);
            item.Name = Convert.ToString(reader["Name"]);

            return item;
        }

        #endregion
    }
}