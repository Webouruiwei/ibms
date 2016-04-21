using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class AreaRepository : AbstractRespository<int, Area>
    {
        public override DbSet<Area> GetAll()
        {
            return db.Areas;
        }

        public override bool IsExist(int id)
        {
            return db.Areas.Count(e => e.ID == id) > 0;
        }
    }
}