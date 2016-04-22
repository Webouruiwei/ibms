using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class PointRepository : AbstractRespository<int, Point>, IPointRepository
    {
        public override DbSet<Point> GetAll()
        {
            return db.Points;
        }

        public override bool IsExist(int uuid)
        {
            return db.Points.Count(e => e.ID == uuid) > 0;
        }

        public override IEnumerable<Point> GetPagerItems<TKey>(int pageIndex, int pageSize, Func<Point, TKey> orderFunc, bool isDesc)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetOriginalAll().OrderBy(orderFunc).Skip(recordStart).Take(pageSize);
            else
                return GetOriginalAll().OrderByDescending(orderFunc).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<Point> GetOriginalAll()
        {
            return GetAll().Where(u => (u.ArchiveTag ?? false) == false);
        }

        public int GetOriginalCount()
        {
            return GetOriginalAll().Count();
        }

        public IEnumerable<Point> GetAll(string moduleID, int areaID, string floor)
        {
            IEnumerable<Point> points = GetOriginalAll();
            if (!string.IsNullOrEmpty(moduleID))
                points = points.Where(u => u.ModuleID == moduleID);
            if (areaID > 0)
                points = points.Where(u => u.AreaID == areaID);
            if (!string.IsNullOrEmpty(floor))
                points = points.Where(u => u.Floor == floor);
            return points;
        }
    }
}