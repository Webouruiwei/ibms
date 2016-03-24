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
    public class RolesController : ApiController
    {
        IRoleRepository roleRepository;
        IRepository<string, Module> moduleRepository;

        public RolesController(IRoleRepository roleRepository, IRepository<string, Module> moduleRepository)
        {
            this.roleRepository = roleRepository;
            this.moduleRepository = moduleRepository;
        }

        // GET: api/Roles
        public IHttpActionResult GetRoles()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var results = this.roleRepository.GetAll();
            var items = from item in results
                        group item by new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            Remark = item.Remark,
                        } into newItem
                        select new
                        {
                            UUID = newItem.Key.UUID,
                            Name = newItem.Key.Name,
                            Description = newItem.Key.Description,
                            Status = newItem.Key.Status,
                            Remark = newItem.Key.Remark,
                        };

            return Ok(items);
        }

        // GET: api/Roles/5
        [ResponseType(typeof(Role))]
        [Route("api/roles/{id:int}")]
        public async Task<IHttpActionResult> GetRole(int id)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Role role = await this.roleRepository.GetByIdAsync(id);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        // GET: api/Roles/9a003d14b1844f3892c44efe08d12593
        public IHttpActionResult GetRole(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<dynamic> roles = this.roleRepository.FindRolesByUUID(uuid);
            if (roles == null)
                return NotFound();

            return Ok(roles);
        }

        /// <summary>
        /// 获取指定角色可供选择的其他模块系统。
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [Route("api/roles/additional_modules/{uuid}")]
        [HttpGet]
        public IHttpActionResult GetRoleAdditionModules(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<Module> modules = this.moduleRepository.GetAll();
            IEnumerable<dynamic> roles = this.roleRepository.FindRolesByUUID(uuid);
            List<dynamic> additions = new List<dynamic>();
            foreach (Module module in modules)
            {
                bool status = module.Status ?? false;
                if (!status)
                    continue;

                bool isContain = false;
                foreach (var role in roles)
                {
                    if (module.UUID == role.ModuleID)
                    {
                        isContain = true;
                        break;
                    }
                }
                if (!isContain)
                    additions.Add(new { ModuleID = module.UUID, ModuleName = module.Name });
            }

            return Ok(additions);
        }

        // PUT: api/Roles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRole(int id, Role role)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (id != role.ID)
                return BadRequest();

            try
            {
                await this.roleRepository.PutAsync(role);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.roleRepository.IsExist(id))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Roles
        public async Task<IHttpActionResult> PostRole()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string Name = HttpContext.Current.Request.Params["Name"];
            string Description = HttpContext.Current.Request.Params["Description"];
            string Modules = HttpContext.Current.Request.Params["Modules"];
            string UUID = TextHelper.GenerateUUID();
            string[] moduleAry = Modules.Split(',');
            foreach (string item in moduleAry)
            {
                string[] itemAry = item.Split(':');
                Role role = new Role();
                role.UUID = UUID;
                role.Name = Name;
                role.Description = Description;
                role.ModuleID = itemAry[0];
                role.CanRead = Convert.ToBoolean(itemAry[1]);
                role.CanWrite = Convert.ToBoolean(itemAry[2]);
                role.Status = true;
                role.CreateTime = DateTime.Now;
                if (!role.CanRead && !role.CanWrite)
                    continue;
                await this.roleRepository.AddAsync(role);
            }

            return Ok();
        }

        [Route("api/roles/add_module")]
        [HttpPost]
        public async Task<IHttpActionResult> PostRoleModule([FromUri] Role roleModule)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            await this.roleRepository.AddAsync(roleModule);

            return Ok();
        }

        // DELETE: api/Roles/5
        [Route("api/roles/{id:int}")]
        public async Task<IHttpActionResult> DeleteRole(int id)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Role role = await this.roleRepository.GetByIdAsync(id);
            if (role == null)
                return NotFound();

            await this.roleRepository.DeleteAsync(role);

            return Ok();
        }

        /// <summary>
        /// 修改角色基本信息（主要是名称和描述）。
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [Route("api/roles/basic/{uuid}")]
        [HttpPost]
        public async Task<IHttpActionResult> ModifyRoleBasic(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var roles = this.roleRepository.FindRolesByUUID(uuid);
            if (roles == null)
                return NotFound();

            string Name = HttpContext.Current.Request.Params["Name"];
            string Description = HttpContext.Current.Request.Params["Description"];
            await this.roleRepository.ModifyRolesBasicAsync(roles, Name, Description);

            return Ok();
        }

        /// <summary>
        /// 修改角色状态。
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [Route("api/roles/status/{uuid}")]
        [HttpPost]
        public async Task<IHttpActionResult> ModifyRoleStatus(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var roles = this.roleRepository.FindRolesByUUID(uuid);
            if (roles == null)
                return NotFound();

            bool Status = Convert.ToBoolean(HttpContext.Current.Request.Params["Status"]);
            await this.roleRepository.ModifyRolesStatusAsync(roles, Status);

            return Ok();
        }
    }
}