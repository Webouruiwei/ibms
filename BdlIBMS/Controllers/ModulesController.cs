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

            var modules = this.repository.GetAll();
            var items = from item in modules
                        select new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            Remark = item.Remark
                        };
            return Ok(items);
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
                Remark = module.Remark
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

            module.Status = false; // 设置该模块不可用
            await this.repository.PutAsync(module);

            return Ok();
        }

        [Route("api/modules/reset/{uuid}")]
        [ResponseType(typeof(Module))]
        public async Task<IHttpActionResult> ResetModule(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Module module = await this.repository.GetByIdAsync(uuid);
            if (module == null)
                return NotFound();

            module.Status = true; // 重置该模块为可用状态
            await this.repository.PutAsync(module);

            return Ok(module);
        }
    }
}