using BdlIBMS.Models;
using BdlIBMS.Repositories;
using BdlIBMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BdlIBMS.Controllers
{
    public class LoginRecordsController : ApiController
    {
        IRepository<int, LoginRecord> repository;

        public LoginRecordsController(IRepository<int, LoginRecord> repository)
        {
            this.repository = repository;
        }

        // GET: api/loginRecords
        public IHttpActionResult GetLoginRecords()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<LoginRecord> loginRecords;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                loginRecords = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                loginRecords = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }
            pager.Items = loginRecords;

            return Ok(pager);
        }

        // GET: api/loginRecords/1
        [ResponseType(typeof(LoginRecord))]
        public async Task<IHttpActionResult> GetLoginRecord(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            LoginRecord loginRecord = await this.repository.GetByIdAsync(uuid);
            if (loginRecord == null)
                return NotFound();

            return Ok(loginRecord);
        }
    }
}
