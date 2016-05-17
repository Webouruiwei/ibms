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
    public class OperationRecordsController : ApiController
    {
        IRepository<int, OperationRecord> repository;

        public OperationRecordsController(IRepository<int, OperationRecord> repository)
        {
            this.repository = repository;
        }

        // GET: api/operationRecords
        public IHttpActionResult GetOperationRecords()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<OperationRecord> operationRecords;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                operationRecords = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                operationRecords = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }
            pager.Items = operationRecords;

            return Ok(pager);
        }

        // GET: api/operationRecords/1
        [ResponseType(typeof(OperationRecord))]
        public async Task<IHttpActionResult> GetOperationRecord(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            OperationRecord operationRecord = await this.repository.GetByIdAsync(uuid);
            if (operationRecord == null)
                return NotFound();

            return Ok(operationRecord);
        }
    }
}
