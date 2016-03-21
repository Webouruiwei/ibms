using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class ModuleRepository : AbstractRespository<string, Module>
    {
        public override DbSet<Module> GetAll()
        {
            return db.Modules;
        }

        public override bool IsExist(string uuid)
        {
            return db.Modules.Count(e => e.UUID == uuid) > 0;
        }
    }
}