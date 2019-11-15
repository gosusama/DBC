using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.Common
{
    public class GrantGroupObj
    {
        public class GroupObj
        {
            public GroupObj()
            {
                Users = new List<User>();
            }
            public string IdGroup { get; set; }
            public string NameGroup { get; set; }
            public string Description { get; set; }
            public List<User> Users { get; set; }
        }

       

        public class User
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string IdProfile { get; set; }
            public string Password { get; set; }
            public string UnitCode { get; set; }
        }


    }
}
