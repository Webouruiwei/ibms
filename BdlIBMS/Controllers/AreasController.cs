using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BdlIBMS.Models;
using BdlIBMS.Repositories;
using BdlIBMS.Utils;

namespace BdlIBMS.Controllers
{
    public class AreasController : ApiController
    {
        IRepository<int, Area> repository;

        public AreasController(IRepository<int, Area> repository)
        {
            this.repository = repository;
        }

        // GET: api/areas
        public IHttpActionResult GetAreas()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Area> areas;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                areas = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                areas = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in areas
                        select new
                        {
                            ID = item.ID,
                            Name = item.Name,
                            Description = item.Description,
                            ParentID = item.ParentID,
                            ParentName = (item.ParentID == null ? "" : this.repository.GetByID(item.ParentID.Value).Name),
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/areas/1
        [ResponseType(typeof(Area))]
        public async Task<IHttpActionResult> GetArea(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Area area = await this.repository.GetByIdAsync(uuid);
            if (area == null)
                return NotFound();

            var result = new
            {
                ID = area.ID,
                Name = area.Name,
                Description = area.Description,
                ParentID = area.ParentID,
                ParentName = (area.ParentID == null ? "" : this.repository.GetByID(area.ParentID.Value).Name),
                CreateTime = area.CreateTime,
                Remark = area.Remark
            };

            return Ok(result);
        }

        // PUT: api/areas/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutArea(int uuid, [FromUri]Area area)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != area.ID)
                return BadRequest();

            try
            {
                area.CreateTime = DateTime.Now;
                await this.repository.PutAsync(area);
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

        // POST: api/areas
        [ResponseType(typeof(Area))]
        public async Task<IHttpActionResult> PostArea([FromUri]Area area)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                area.CreateTime = DateTime.Now;
                await this.repository.AddAsync(area);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(area.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/areas/1
        public async Task<IHttpActionResult> DeleteArea(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Area area = await this.repository.GetByIdAsync(uuid);
            if (area == null)
                return NotFound();

            await this.repository.DeleteAsync(area);

            return Ok();
        }
    }
}