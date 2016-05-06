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
    public class BuildingsController : ApiController
    {
        IRepository<int, Building> repository;

        public BuildingsController(IRepository<int, Building> repository)
        {
            this.repository = repository;
        }

        // GET: api/buildings
        public IHttpActionResult GetBuildings()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Building> buildings;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                buildings = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                buildings = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in buildings
                        select new
                        {
                            ID = item.ID,
                            Name = item.Name,
                            Description = item.Description,
                            FloorStart = item.FloorStart,
                            FloorEnd = item.FloorEnd,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/buildings/1
        [ResponseType(typeof(Building))]
        public async Task<IHttpActionResult> GetBuilding(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Building building = await this.repository.GetByIdAsync(uuid);
            if (building == null)
                return NotFound();

            var result = new
            {
                ID = building.ID,
                Name = building.Name,
                Description = building.Description,
                FloorStart = building.FloorStart,
                FloorEnd = building.FloorEnd,
                CreateTime = building.CreateTime,
                Remark = building.Remark
            };

            return Ok(result);
        }

        // PUT: api/buildings/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBuilding(int uuid, [FromUri]Building building)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != building.ID)
                return BadRequest();

            try
            {
                building.CreateTime = DateTime.Now;
                await this.repository.PutAsync(building);
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

        // POST: api/buildings
        [ResponseType(typeof(Building))]
        public async Task<IHttpActionResult> PostBuilding([FromUri]Building building)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                building.CreateTime = DateTime.Now;
                await this.repository.AddAsync(building);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(building.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/buildings/1
        public async Task<IHttpActionResult> DeleteBuilding(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Building building = await this.repository.GetByIdAsync(uuid);
            if (building == null)
                return NotFound();

            await this.repository.DeleteAsync(building);

            return Ok();
        }
    }
}