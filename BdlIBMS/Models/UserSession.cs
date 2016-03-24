using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BdlIBMS.Models
{
    public class UserSession
    {
        public string UUID { get; set; }

        public string RoleID { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public bool? Sex { get; set; }

        public string Company { get; set; }

        public string Position { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
    }
}