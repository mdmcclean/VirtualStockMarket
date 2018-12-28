using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone
{
    public class UserController : StockGameBaseController
    {
        private IStockGameDAL _dal;

        public UserController(IStockGameDAL dal) : base(dal)
        {
            _dal = dal;
        }
        // GET: User
        public ActionResult Landing()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (base.IsAuthenticated)
            {
                LogUserOut();
            }

            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            ActionResult result = null;

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                UserItem user = null;
                try
                {
                    user = _dal.GetUserItem(model.Username);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("invalid-user", "Either the username or the password is invalid.");
                    throw;
                }

                PasswordHelper passHelper = new PasswordHelper(model.Password, user.Salt);
                if (!passHelper.Verify(user.Hash))
                {
                    ModelState.AddModelError("invalid-user", "Either the username or the password is invalid.");
                    throw new Exception();
                }

                // Happy Path
                base.LogUserIn(user);
                string isSetting = _dal.CheckSetting();
                if (isSetting == "0")
                {
                    bool okay = _dal.SwitchSettings(2);
                    result = RedirectToAction("Settings", "StockGame");
                }
                else
                {
                    result = RedirectToAction("Game", "StockGame");
                }
            }
            catch (Exception)
            {
                result = View("Login");
            }

            return result;
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (base.IsAuthenticated)
            {
                LogUserOut();
            }

            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            ActionResult result = null;

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                UserItem user = null;
                try
                {
                    user = _dal.GetUserItem(model.Username);
                }
                catch (Exception)
                {
                }

                if (user != null)
                {
                    ModelState.AddModelError("invalid-user", "The username is already taken.");
                    throw new Exception();
                }

                PasswordHelper passHelper = new PasswordHelper(model.Password);
                UserItem newUser = new UserItem()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    Salt = passHelper.Salt,
                    Hash = passHelper.Hash,
                    RoleId = (int)RoleManager.eRole.Player
                };

                _dal.AddUserItem(newUser);
                base.LogUserIn(newUser);

                var Model = Session[CurrentUserSession] as UserItem;
                if (Model.Id == 1)
                {
                    result = RedirectToAction("Settings", "StockGame");
                }
                else
                {
                    result = RedirectToAction("Game", "StockGame");
                }
            }
            catch (Exception)
            {
                result = View("Register");
            }

            return result;
        }
    }
}