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
        IRoleRepository repository;

        public RolesController(IRoleRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/Roles
        public IHttpActionResult GetRoles()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var results = this.repository.GetAll();
            var items = from item in results
                        group item by new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            Remark = item.Remark
                        } into newItem
                        select new
                        {
                            UUID = newItem.Key.UUID,
                            Name = newItem.Key.Name,
                            Description = newItem.Key.Description,
                            Status = newItem.Key.Status,
                            Remark = newItem.Key.Remark
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

            Role role = await this.repository.GetByIdAsync(id);
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

            IEnumerable<dynamic> roles = this.repository.FindRolesByUUID(uuid);
            if (roles == null)
                return NotFound();

            return Ok(roles);
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
                await this.repository.PutAsync(role);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.repository.IsExist(id))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Roles
        [ResponseType(typeof(Role))]
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
                if (!role.CanRead && !role.CanWrite)
                    continue;
                await this.repository.AddAsync(role);
            }

            return Ok();
        }

        // DELETE: api/Roles/5
        [Route("api/roles/{id:int}")]
        public async Task<IHttpActionResult> DeleteRole(int id)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Role role = await this.repository.GetByIdAsync(id);
            if (role == null)
                return NotFound();

            role.Status = false;
            await this.repository.PutAsync(role);

            return Ok();
        }

        // DELETE: api/Roles/9a003d14b1844f3892c44efe08d12593
        public async Task<IHttpActionResult> DeleteRole(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var roles = this.repository.FindRolesByUUID(uuid);
            await this.repository.DeleteRolesAsync(roles);

            return Ok();
        }
    }
}