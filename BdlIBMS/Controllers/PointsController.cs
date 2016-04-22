using BdlIBMS.Models;
using BdlIBMS.Repositories;
using BdlIBMS.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BdlIBMS.Controllers
{
    public class PointsController : ApiController
    {
        IPointRepository repository;

        public PointsController(IPointRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/points
        public IHttpActionResult GetPoints()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Point> points;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                points = this.repository.GetOriginalAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetOriginalCount());
                points = this.repository.GetPagerItems(pageIndex, pageSize, u => u.PointID);
            }

            var items = from item in points
                        select new
                        {
                            ID = item.ID,
                            PointID = item.PointID,
                            ModuleID = item.ModuleID,
                            ModuleName = item.Module.Name,
                            Protocol = item.Protocol,
                            AreaID = item.AreaID,
                            AreaName = item.Area.Name,
                            Floor = item.Floor,
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDescription,
                            ValueFunc = item.ValueFunc,
                            MinValue = item.MinValue,
                            MaxValue = item.MaxValue,
                            Type = item.Type,
                            Unit = item.Unit,
                            IsArchive = item.IsArchive,
                            ArchiveInterval = item.ArchiveInterval,
                            ParentID = item.ParentID
                        };
            pager.Items = items;

            return Ok(pager);
        }

        [Route("api/points/factors")]
        [HttpGet]
        public IHttpActionResult GetPointsByFactors()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string ModuleID = HttpContext.Current.Request.Params["ModuleID"];
            int AreaID = Convert.ToInt32(HttpContext.Current.Request.Params["AreaID"]);
            string Floor = HttpContext.Current.Request.Params["Floor"];
            IEnumerable<Point> points = this.repository.GetAll(ModuleID, AreaID, Floor);
            var items = from item in points
                        select new
                        {
                            ID = item.ID,
                            PointID = item.PointID,
                            ModuleID = item.ModuleID,
                            ModuleName = item.Module.Name,
                            Protocol = item.Protocol,
                            AreaID = item.AreaID,
                            AreaName = item.Area.Name,
                            Floor = item.Floor,
                            TopPos = item.TopPos,
                            LeftPos = item.LeftPos,
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDescription,
                            ValueFunc = item.ValueFunc,
                            Value = item.Value,
                            MinValue = item.MinValue,
                            MaxValue = item.MaxValue,
                            Type = item.Type,
                            Unit = item.Unit,
                            DateTime = item.DateTime,
                            IsArchive = item.IsArchive,
                            ArchiveInterval = item.ArchiveInterval,
                            ParentID = item.ParentID
                        };

            return Ok(items);
        }

        // GET: api/points/400001
        [ResponseType(typeof(Point))]
        public async Task<IHttpActionResult> GetPoint(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Point item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                PointID = item.PointID,
                ModuleID = item.ModuleID,
                ModuleName = item.Module.Name,
                Protocol = item.Protocol,
                AreaID = item.AreaID,
                AreaName = item.Area.Name,
                Floor = item.Floor,
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                ItemDescription = item.ItemDescription,
                ValueFunc = item.ValueFunc,
                MinValue = item.MinValue,
                MaxValue = item.MaxValue,
                Type = item.Type,
                Unit = item.Unit,
                IsArchive = item.IsArchive,
                ArchiveInterval = item.ArchiveInterval,
                ParentID = item.ParentID
            };

            return Ok(result);
        }

        // PUT: api/points/400001
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPoint(int uuid, [FromUri]Point point)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != point.ID)
                return BadRequest();

            try
            {
                await this.repository.PutAsync(point);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.repository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/points/position/{uuid}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutPointPostion(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (!this.repository.IsExist(uuid))
                return NotFound();

            double Left = Convert.ToDouble(HttpContext.Current.Request.Params["Left"]);
            double Top = Convert.ToDouble(HttpContext.Current.Request.Params["Top"]);
            Point point = await this.repository.GetByIdAsync(uuid);
            point.LeftPos = Left;
            point.TopPos = Top;
            try
            {
                await this.repository.PutAsync(point);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/points
        [ResponseType(typeof(Point))]
        public async Task<IHttpActionResult> PostPoint([FromUri]Point point)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                point.TopPos = 0;
                point.LeftPos = 0;
                point.Status = false;
                point.DateTime = DateTime.Now;
                point.ArchiveTime = DateTime.Now;
                point.ArchiveTag = false;
                await this.repository.AddAsync(point);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(point.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/points/400001
        public async Task<IHttpActionResult> DeletePoint(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Point point = await this.repository.GetByIdAsync(uuid);
            if (point == null)
                return NotFound();

            await this.repository.DeleteAsync(point);

            return Ok();
        }
    }
}
