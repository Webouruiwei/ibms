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
        IEnumerable<Point> GetOriginalAll();

        int GetOriginalCount();
    }
}
