using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone
{
    public class UserItem
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public int RoleId { get; set; }
        public bool isReady { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}