using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BdlIBMS.Models;
using BdlIBMS.Repositories;
using BdlIBMS.Utils;

namespace BdlIBMS.Controllers
{
    public class UserInfoesController : ApiController
    {
        IRepository<string, UserInfo> repository;

        public UserInfoesController(IRepository<string, UserInfo> repository)
        {
            this.repository = repository;
        }

        // GET: api/UserInfoes
        [ResponseType(typeof(UserInfo))]
        public IHttpActionResult GetUserInfoes()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            return Ok(this.repository.GetAll());
        }

        // GET: api/UserInfoes/5
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> GetUserInfo(string uuid)
        {
            UserInfo userInfo = await this.repository.GetByIdAsync(uuid);
            if (userInfo == null)
                return NotFound();

            return Ok(userInfo);
        }

        // PUT: api/UserInfoes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserInfo(string uuid, UserInfo userInfo)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != userInfo.UUID)
                return BadRequest();

            try
            {
                await this.repository.PutAsync(userInfo);
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

        // POST: api/UserInfoes
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> PostUserInfo(UserInfo userInfo)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                await this.repository.AddAsync(userInfo);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(userInfo.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok(userInfo);
        }

        // DELETE: api/UserInfoes/5
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> DeleteUserInfo(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            UserInfo userInfo = await this.repository.GetByIdAsync(uuid);
            if (userInfo == null)
                return NotFound();

            await this.repository.DeleteAsync(userInfo);

            return Ok(userInfo);
        }
    }
}