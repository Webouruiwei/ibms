using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class OperationRecordRepository : AbstractRespository<int, OperationRecord>
    {
        public override DbSet<OperationRecord> GetAll()
        {
            return db.OperationRecords;
        }

        public override bool IsExist(int uuid)
        {
            return db.OperationRecords.Count(e => e.ID == uuid) > 0;
        }
    }
}