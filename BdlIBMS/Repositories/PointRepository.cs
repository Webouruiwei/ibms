using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class PointRepository : AbstractRespository<int, Point>, IPointRepository
    {
        public Point GetByPointID(string pointID)
        {
            return (from item in GetAll() where item.PointID == pointID select item).FirstOrDefault();
        }

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
            if (floor != null)
                points = points.Where(u => u.Floor == floor);
            return points;
        }

        public IEnumerable<TrendData> GetTrendData(string pointID, DateTime startTime, DateTime endTime)
        {
            string strSql = "";
            TimeSpan span = endTime - startTime;
            double days = span.TotalDays;
            if (days > 365) // 按年统计
            {
                strSql = "SELECT DATEPART(YYYY,DateTime) AS Timeline, AVG(CONVERT(decimal(18, 6),Value)) AS Valueline FROM Point ";
                strSql += "WHERE PointID= @PointID AND ArchiveTag='1' AND DateTime BETWEEN @StartTime AND @EndTime GROUP BY DATEPART(YYYY,DateTime);";
            }
            else if (days > 31) // 按月统计
            {
                strSql = "SELECT CONVERT(VARCHAR(7),DateTime,25) AS Timeline, AVG(CONVERT(decimal(18, 6),Value)) AS Valueline FROM Point ";
                strSql += "WHERE PointID= @PointID AND ArchiveTag='1' AND DateTime BETWEEN @StartTime AND @EndTime GROUP BY CONVERT(VARCHAR(7),DateTime,25);";
            }
            else if (days > 1 && days <= 31) // 按天统计
            {
                strSql = "SELECT CONVERT(VARCHAR(10),DateTime,25) AS Timeline, AVG(CONVERT(decimal(18, 6),Value)) AS Valueline FROM Point ";
                strSql += "WHERE PointID= @PointID AND ArchiveTag='1' AND DateTime BETWEEN @StartTime AND @EndTime GROUP BY CONVERT(VARCHAR(10),DateTime,25);";
            }
            else if (span.TotalHours > 1 && span.TotalHours <= 24) // 按小时统计
            {
                strSql = "SELECT CONVERT(varchar(10),DATEPART(HH,DateTime)) AS Timeline, AVG(CONVERT(decimal(18, 6),Value)) AS Valueline FROM Point ";
                strSql += "WHERE PointID= @PointID AND ArchiveTag='1' AND DateTime BETWEEN @StartTime AND @EndTime GROUP BY DATEPART(HH,DateTime);";
            }
            else if (span.TotalMinutes > 1 && span.TotalMinutes <= 60) // 按分钟统计
            {
                strSql = "SELECT CONVERT(varchar(10),DATEPART(MI,DateTime)) AS Timeline, AVG(CONVERT(decimal(18, 6),Value)) AS Valueline FROM Point ";
                strSql += "WHERE PointID= @PointID AND ArchiveTag='1' AND DateTime BETWEEN @StartTime AND @EndTime GROUP BY DATEPART(MI,DateTime);";
            }

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@PointID", pointID),
                new SqlParameter("@StartTime",startTime),
                new SqlParameter("@EndTime",endTime)
            };
            IEnumerable<TrendData> result = db.Database.SqlQuery<TrendData>(strSql, parameters);
            return result;
        }
    }
}