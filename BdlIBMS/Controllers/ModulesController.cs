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
    public class ModulesController : ApiController
    {
        IRepository<string, Module> repository;

        public ModulesController(IRepository<string, Module> repository)
        {
            this.repository = repository;
        }

        // GET: api/Modules
        public IHttpActionResult GetModules()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Module> modules;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                modules = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                modules = this.repository.GetPagerItems(pageIndex, pageSize, u => u.CreateTime);
            }

            var items = from item in modules
                        select new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            RefreshInterval = item.RefreshInterval,
                            Remark = item.Remark,
                            CreateTime = item.CreateTime
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/Modules/13cbc2e968bb42a5a60a59742c8684cc
        [ResponseType(typeof(Module))]
        public async Task<IHttpActionResult> GetModule(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Module module = await this.repository.GetByIdAsync(uuid);
            if (module == null)
                return NotFound();

            var result = new
            {
                UUID = module.UUID,
                Name = module.Name,
                Description = module.Description,
                Status = module.Status,
                RefreshInterval = module.RefreshInterval,
                Remark = module.Remark,
                CreateTime = module.CreateTime
            };

            return Ok(result);
        }

        // PUT: api/Modules/13cbc2e968bb42a5a60a59742c8684cc
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutModule(string uuid, [FromUri]Module module)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != module.UUID)
                return BadRequest();

            try
            {
                module.Status = true;
                await this.repository.PutAsync(module);
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

        // POST: api/Modules
        [ResponseType(typeof(Module))]
        public async Task<IHttpActionResult> PostModule([FromUri]Module module)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            module.UUID = BdlIBMS.Utils.TextHelper.GenerateUUID();
            module.Status = true; // 默认添加的时候该模块是可用的
            module.CreateTime = DateTime.Now;
            try
            {
                await this.repository.AddAsync(module);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(module.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/Modules/13cbc2e968bb42a5a60a59742c8684cc
        public async Task<IHttpActionResult> DeleteModule(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Module module = await this.repository.GetByIdAsync(uuid);
            if (module == null)
                return NotFound();

            await this.repository.DeleteAsync(module);

            return Ok();
        }

        [Route("api/modules/status/{uuid}")]
        [HttpPost]
        public async Task<IHttpActionResult> ModifyModuleStatus(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Module module = await this.repository.GetByIdAsync(uuid);
            if (module == null)
                return NotFound();

            bool Status = Convert.ToBoolean(HttpContext.Current.Request.Params["Status"]);
            module.Status = Status; // 修改该模块可用状态
            await this.repository.PutAsync(module);

            return Ok();
        }
    }
}