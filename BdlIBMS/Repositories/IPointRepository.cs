using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdlIBMS.Repositories
{
    public interface IPointRepository : IRepository<int, Point>
    {
        Point GetByPointID(string pointID); 

        IEnumerable<Point> GetOriginalAll();

        int GetOriginalCount();

        IEnumerable<Point> GetAll(string moduleID, int areaID, string floor, bool isArchive);

        IEnumerable<TrendData> GetTrendData(string pointID, DateTime startTime, DateTime endTime);
    }
}
