using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class BuildingRepository : AbstractRespository<int, Building>
    {
        public override DbSet<Building> GetAll()
        {
            return db.Buildings;
        }

        public override bool IsExist(int id)
        {
            return db.Buildings.Count(e => e.ID == id) > 0;
        }
    }
}