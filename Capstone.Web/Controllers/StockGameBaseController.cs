using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone
{
    public class StockGameBaseController : Controller
    {
        //private const string InvMgrKey = "InventoryManagerKey";
        //private const string RepMgrKey = "ReportManagerKey";
        //private const string TransMgrKey = "TransactionManagerKey";
        public const string RoleMgrKey = "RoleManagerKey";
        public const string CurrentUserSession = "CurrentUser";

        private IStockGameDAL _dal;

        public StockGameBaseController(IStockGameDAL dal)
        {
            _dal = dal;
        }

        public ActionResult GetAuthenticatedView(string viewName, object model = null)
        {
            ActionResult result = null;
            if (IsAuthenticated)
            {
                result = View(viewName, model);
            }
            else
            {
                result = RedirectToAction("Login", "User");
            }
            return result;
        }

        public JsonResult GetAuthenticatedJson(JsonResult json, bool hasPermission)
        {
            JsonResult result = null;
            if (!hasPermission && IsAuthenticated)
            {
                result = Json(new { error = "User is not permitted to access this data." }, JsonRequestBehavior.AllowGet);
            }
            else if (IsAuthenticated)
            {
                result = json;
            }
            else
            {
                result = Json(new { error = "User is not authenticated." }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        /// <summary>
        /// Returns bool if user has authenticated in
        /// </summary>        
        public bool IsAuthenticated
        {
            get
            {
                return GetRoleMgr().User != null;
            }
        }

        /// <summary>
        /// Returns the current authenticated user
        /// </summary>        
        public UserItem CurrentUser
        {
            get
            {
                return GetRoleMgr().User;
            }
        }

        /// <summary>
        /// "Logs" the current user in
        /// </summary>
        public void LogUserIn(UserItem user)
        {
            UpdateRoleManager(user);
            Session[CurrentUserSession] = user;
        }

        /// <summary>
        /// "Logs out" a user by removing the cookie.
        /// </summary>
        public void LogUserOut()
        {
            UpdateRoleManager(null);
        }

        /// <summary>
        /// Gets the inventory manager object from the session
        /// </summary>
        /// <returns>InventoryManager</returns>
        //public InventoryManager GetInvMgr()
        //{
        //    InventoryManager inv = Session[InvMgrKey] as InventoryManager;

        //    if (inv == null)
        //    {
        //        var items = _db.GetVendingItems();
        //        inv = new InventoryManager(items, _db);
        //        Session[InvMgrKey] = inv;
        //    }

        //    return inv;
        //}

        /// <summary>
        /// Gets the report manager object from the session
        /// </summary>
        /// <returns>ReportManager</returns>
        //public ReportManager GetReportMgr()
        //{
        //    ReportManager report = Session[RepMgrKey] as ReportManager;

        //    if (report == null)
        //    {
        //        report = new ReportManager(_db);
        //        Session[RepMgrKey] = report;
        //    }

        //    return report;
        //}

        /// <summary>
        /// Gets the transaction manager object from the session
        /// </summary>
        /// <returns>TransactionManager</returns>
        //public TransactionManager GetTransMgr()
        //{
        //    TransactionManager trans = Session[TransMgrKey] as TransactionManager;

        //    if (trans == null)
        //    {
        //        trans = new TransactionManager(_db, _log);
        //        Session[TransMgrKey] = trans;
        //    }

        //    return trans;
        //}

        /// <summary>
        /// Gets the role manager object from the session
        /// </summary>
        /// <returns>RoleManager</returns>
        public RoleManager GetRoleMgr()
        {
            RoleManager roleMgr = Session[RoleMgrKey] as RoleManager;

            if (roleMgr == null)
            {
                roleMgr = UpdateRoleManager(null);
            }

            return roleMgr;
        }

        private RoleManager UpdateRoleManager(UserItem user)
        {
            RoleManager roleMgr = new RoleManager(user);
            Session[RoleMgrKey] = roleMgr;
            return roleMgr;
        }
    }
}