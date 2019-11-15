using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.Common
{
    public class GrantInfoObj
    {
        public class Menu
        {
            public Menu()
            {
                Users = new List<User>();
                Roles = new List<int>();
            }

            public string MenuIdCha { get; set; }
            public string MenuId { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }

            public List<User> Users { get; set; }
            public List<int> Roles { get; set; }
        }

       

        public class User
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string MaDonVi { get; set; }
            public string MaCuaHang { get; set; }
        }


    }
}
