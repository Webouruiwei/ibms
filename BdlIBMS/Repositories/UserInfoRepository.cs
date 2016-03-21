using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class UserInfoRepository : AbstractRespository<string, UserInfo>
    {
        public override DbSet<UserInfo> GetAll()
        {
            return db.UserInfoes;
        }

        public override bool IsExist(string uuid)
        {
            return db.UserInfoes.Count(e => e.UUID == uuid) > 0;
        }
    }
}