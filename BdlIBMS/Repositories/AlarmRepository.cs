using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class AlarmRepository : AbstractRespository<int, Alarm>
    {
        public override DbSet<Alarm> GetAll()
        {
            return db.Alarms;
        }

        public override bool IsExist(int id)
        {
            return db.Alarms.Count(e => e.ID == id) > 0;
        }
    }
}