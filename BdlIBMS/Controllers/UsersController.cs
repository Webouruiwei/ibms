﻿using System;
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
    public class UsersController : ApiController
    {
        private class UserDTO
        {
            public string UUID { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string RoleID { get; set; }
            public string RoleName { get; set; }
            public bool Status { get; set; }
            public string Remark { get; set; }
            public DateTime? CreateTime { get; set; }
        }

        IUserRepository userRepository;
        IRepository<string, UserInfo> userInfoRepository;
        IRoleRepository roleRepository;
        IRepository<int, LoginRecord> loginRecordRepository;

        public UsersController(
            IUserRepository userRepository,
            IRepository<string, UserInfo> userInfoRepository,
            IRoleRepository roleRepository,
            IRepository<int, LoginRecord> loginRecordRepository)
        {
            this.userRepository = userRepository;
            this.userInfoRepository = userInfoRepository;
            this.roleRepository = roleRepository;
            this.loginRecordRepository = loginRecordRepository;
        }

        // GET: api/Users
        public IHttpActionResult GetUsers()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            List<UserDTO> list = new List<UserDTO>();
            var users = this.userRepository.GetAll();
            foreach (var item in users)
            {
                UserDTO user = new UserDTO();
                user.UUID = item.UUID;
                user.UserName = item.UserName;
                user.Password = item.Password;
                user.RoleName = this.roleRepository.FindRoleNameByUUID(item.RoleID);
                user.Status = item.Status ?? true;
                user.Remark = item.Remark;
                user.CreateTime = item.CreateTime;
                list.Add(user);
            }

            return Ok(list);
        }

        // GET: api/Users/561169739ed84672a49edadb45d01000
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> GetUser(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            User user = await this.userRepository.GetByIdAsync(uuid);
            if (user == null)
                return NotFound();

            UserDTO userDto = new UserDTO();
            userDto.UUID = user.UUID;
            userDto.UserName = user.UserName;
            userDto.Password = user.Password;
            userDto.RoleID = user.RoleID;
            userDto.RoleName = this.roleRepository.FindRoleNameByUUID(user.RoleID);
            userDto.Status = user.Status ?? true;
            userDto.Remark = user.Remark;
            userDto.CreateTime = user.CreateTime;

            return Ok(userDto);
        }

        [Route("api/user")]
        [HttpGet]
        [ResponseType(typeof(UserSession))]
        public IHttpActionResult GetUserSession()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            UserSession session = HttpContext.Current.Session["mySession"] as UserSession;
            return Ok(session);
        }

        [Route("api/users/login")]
        [HttpGet]
        public async Task<IHttpActionResult> Login(string userName, string password)
        {
            User user = this.userRepository.FindByUserNameAndPassword(userName, password);
            LoginRecord loginRecord = new LoginRecord();
            loginRecord.UserName = userName;
            loginRecord.IP = TextHelper.GetHostAddress();
            loginRecord.DateTime = DateTime.Now;
            if (user == null)
            {
                loginRecord.Result = "登录不成功，[User]表中指定用户名不存在或密码错误！";
                await this.loginRecordRepository.AddAsync(loginRecord);
                return NotFound();
            }
                

            UserInfo userInfo = await this.userInfoRepository.GetByIdAsync(user.UUID);
            if (userInfo == null)
            {
                loginRecord.Result = "登录不成功，[UserInfo]表中指定用户名不存在或密码错误！";
                await this.loginRecordRepository.AddAsync(loginRecord);
                return NotFound();
            }

            // 登录成功，记录下用户登录日志
            loginRecord.Result = "恭喜你，登录成功！";
            await this.loginRecordRepository.AddAsync(loginRecord);

            UserSession session = new UserSession();
            session.UUID = user.UUID;
            session.UserName = user.UserName;
            session.RoleID = user.RoleID;
            session.RealName = userInfo.RealName;
            session.Sex = userInfo.Sex;
            session.Company = userInfo.Company;
            session.Position = userInfo.Position;
            session.Phone = userInfo.Phone;
            session.Email = userInfo.Email;
            session.Address = userInfo.Address;
            HttpContext.Current.Session["mySession"] = session;

            return Ok();
        }

        [Route("api/users/logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            HttpContext.Current.Session["mySession"] = null;
            return Ok();
        }

        // PUT: api/Users/561169739ed84672a49edadb45d01000
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string uuid, [FromUri]User user)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != user.UUID)
                return BadRequest();

            try
            {
                user.Password = TextHelper.MD5Encrypt(user.Password);
                user.Status = true;
                await this.userRepository.PutAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.userRepository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser([FromUri]User user)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                user.UUID = TextHelper.GenerateUUID();
                user.Password = TextHelper.MD5Encrypt(user.Password);
                user.Status = true;
                user.CreateTime = DateTime.Now;
                await this.userRepository.AddAsync(user);

                UserInfo userInfo = new UserInfo();
                userInfo.UUID = user.UUID;
                userInfo.RealName = "请填写你的真实名字！";
                userInfo.Sex = true;
                await this.userInfoRepository.AddAsync(userInfo);
            }
            catch (DbUpdateException)
            {
                if (this.userRepository.IsExist(user.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok(user);
        }

        // DELETE: api/Users/561169739ed84672a49edadb45d01000
        public async Task<IHttpActionResult> DeleteUser(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            User user = await this.userRepository.GetByIdAsync(uuid);
            if (user == null)
                return NotFound();

            await this.userRepository.DeleteAsync(user);

            return Ok();
        }

        [Route("api/users/status/{uuid}")]
        [HttpPost]
        public async Task<IHttpActionResult> ModifyUserStatus(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            User user = await this.userRepository.GetByIdAsync(uuid);
            if (user == null)
                return NotFound();

            bool Status = Convert.ToBoolean(HttpContext.Current.Request.Params["Status"]);
            user.Status = Status; // 修改该用户可用状态
            await this.userRepository.PutAsync(user);

            return Ok();
        }

        [Route("api/users/access/{moduleUUID}")]
        [HttpGet]
        public IHttpActionResult CheckUserAccess(string moduleUUID)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            UserSession session = HttpContext.Current.Session["mySession"] as UserSession;
            if (session == null)
                return NotFound();
            IEnumerable<dynamic> roles = this.roleRepository.FindRolesByUUID(session.RoleID);
            var roleAccesses = from item in roles
                               where item.ModuleID == moduleUUID
                               select
                               new
                               {
                                   CanRead = item.CanRead,
                                   CanWrite = item.CanWrite
                               };
            var roleAccess = roleAccesses.FirstOrDefault();
            if (roleAccess == null)
                return Ok(new { CanRead = false, CanWrite = false });

            return Ok(roleAccess);
        }
    }
}