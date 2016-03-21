using BdlIBMS.Models;
using BdlIBMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class UserRepository : AbstractRespository<string, User>, IUserRepository
    {
        public override System.Data.Entity.DbSet<User> GetAll()
        {
            return db.Users;
        }

        public override bool IsExist(string uuid)
        {
            return db.Users.Count(e => e.UUID == uuid) > 0;
        }

        public User FindByUserNameAndPassword(string userName, string password)
        {
            string encryptPwd = TextHelper.MD5Encrypt(password);
            User user = this.db.Users.FirstOrDefault((u) => u.UserName == userName && u.Password == encryptPwd);
            return user;
        }
    }
}