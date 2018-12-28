using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone;

namespace Capstone
{
    public class ReadyUsers
    {
        List<UserItem> _readyUsers = new List<UserItem>();
        public ReadyUsers(List<UserItem> users)
        {
            _readyUsers = users;
        }
    }
}
