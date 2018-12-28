using StockGameService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone
{
    public class StockGameApiController : StockGameBaseController
    {
        private IStockGameDAL _dal;

        public StockGameApiController(IStockGameDAL dal) : base(dal)
        {
            _dal = dal;
        }

        [HttpGet]
        [Route("api/ReadyUsers")]
        public ActionResult GetReadyUsers(int gameId)
        {
            var readyUsers = new ReadyUsers(_dal.UsersPlaying(gameId));
            var jsonResult = Json(readyUsers, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [HttpPost]
        [Route("api/AddUserToGame")]
        public ActionResult AddUserToGame(int userId)
        {
            bool didWork = _dal.AddUserGame(userId);
            var jsonResult = Json(new Game() { GameID = 1}, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [HttpGet]
        [Route("api/ListOfAvailableStocks")]
        public ActionResult GetStocks()
        {
            var availStocks = new AvailableStocks(_dal.AvailableStocks());
            var jsonResult = Json(availStocks, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [HttpGet]
        [Route("api/UserStocks")]
        public ActionResult GetStocksByUser(int userId)
        {
            var userStocks = new UserHoldings(_dal.UserStocks(userId));
            var jsonResult = Json(userStocks, JsonRequestBehavior.AllowGet);
            return jsonResult;
            
        }

        [HttpPost]
        [Route("api/BuyStock")]
        public ActionResult BuyStock(int userId, int stockId, int shares)
        {
            bool isSuccess = false;
            if (shares > 0)
            {
                isSuccess = _dal.AddUserStock(userId, stockId, shares);
            }
            else if(shares < 0)
            {
                isSuccess = _dal.SellStock(userId, stockId, shares*-1);
            }
            JsonResult jsonResult = null;
            if (isSuccess)
            {
                //bool didWork = _dal.UpdateStocks();
                var availStocks = new AvailableStocks(_dal.AvailableStocks());
                jsonResult = Json(availStocks, JsonRequestBehavior.AllowGet);
            }
            return jsonResult;
        }

        [HttpGet]
        [Route("api/Update")]
        public ActionResult UpdateStocks()
        {
            bool didWork = _dal.UpdateStocks();
            var availStocks = new AvailableStocks(_dal.AvailableStocks());
            var jsonResult = Json(availStocks, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }



        [HttpGet]
        [Route("api/GetCashBalances")]
        public ActionResult GetCash()
        {
            List<UserCash> playerCash = new List<UserCash>();
            playerCash = _dal.GetCashAmounts();
            var jsonResult = Json(playerCash, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [HttpGet]
        [Route("api/UserID")]
        public ActionResult GetUserIDFromUsername(string username)
        {
            int id = _dal.GetUserIdByUsername(username);
            UserItem myUser = new UserItem()
            {
                Id = id,
                Username = username
            };
            var jsonResult = Json(myUser, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }

        [HttpGet]
        [Route("api/GetTimeEnd")]
        public ActionResult GetTimeEnd()
        {
            //List<UserCash> playerCash = new List<UserCash>();
            DateTime timeEnd = _dal.TimeEnd();
            //DateTime timeEnd = DateTime.Now;
            timeEnd.ToShortTimeString();
            var jsonResult = Json(timeEnd, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [Route("api/GetOwnersOfStock")]
        public ActionResult GetStockMajorityOwners()
        {
            var owners = _dal.GetOwners();
            var jsonResult = Json(owners, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [HttpPost]
        [Route("api/SwitchSettings")]
        public ActionResult SwitchSetting(int setting)
        {
            bool didWork = _dal.SwitchSettings(setting);
            var jsonResult = Json(new Game() { GameID = 1 }, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        [HttpGet]
        [Route("api/CheckSetting")]
        public ActionResult CheckIfSettings()
        {
            string setting = _dal.CheckSetting();
            CheckSetting theSetting = new CheckSetting();
            theSetting.SettingValue = setting;
            var jsonResult = Json(theSetting, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}