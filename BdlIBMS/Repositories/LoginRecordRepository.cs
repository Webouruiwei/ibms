using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class LoginRecordRepository : AbstractRespository<int, LoginRecord>
    {
        public override DbSet<LoginRecord> GetAll()
        {
            return db.LoginRecords;
        }

        public override bool IsExist(int uuid)
        {
            return db.LoginRecords.Count(e => e.ID == uuid) > 0;
        }
    }
}