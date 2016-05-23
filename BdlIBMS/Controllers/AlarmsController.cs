using BdlIBMS.Models;
using BdlIBMS.Repositories;
using BdlIBMS.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BdlIBMS.Controllers
{
    public class AlarmsController : ApiController
    {
        IRepository<int, Alarm> repository;

        public AlarmsController(IRepository<int, Alarm> repository)
        {
            this.repository = repository;
        }

        // GET: api/alarms
        public IHttpActionResult GetAlarms()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Alarm> alarms;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                alarms = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                alarms = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in alarms
                        select new
                        {
                            ID = item.ID,
                            PointID = item.PointID,
                            ModuleName = item.Point.Module.Name,
                            ItemName = item.Point.ItemName,
                            Priority = item.Priority,
                            Reason = item.Reason,
                            OccurTime = item.OccurTime,
                            IsProcess = item.IsProcess,
                            Principal = item.Principal,
                            ProcessContent = item.ProcessContent,
                            IsSuccess = item.IsSuccess,
                            ProcessTime = item.ProcessTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/alarms/1
        [ResponseType(typeof(Alarm))]
        public async Task<IHttpActionResult> GetAlarm(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Alarm item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                PointID = item.PointID,
                ModuleName = item.Point.Module.Name,
                ItemName = item.Point.ItemName,
                Priority = item.Priority,
                Reason = item.Reason,
                OccurTime = item.OccurTime,
                IsProcess = item.IsProcess,
                Principal = item.Principal,
                ProcessContent = item.ProcessContent,
                IsSuccess = item.IsSuccess,
                ProcessTime = item.ProcessTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/alarms/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAlarm(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string Principal = HttpContext.Current.Request.Params["Principal"];
            string ProcessContent = HttpContext.Current.Request.Params["ProcessContent"];
            bool IsSuccess = Convert.ToBoolean(HttpContext.Current.Request.Params["IsSuccess"]);
            string Remark = HttpContext.Current.Request.Params["Remark"];
            try
            {
                Alarm alarm = this.repository.GetByID(uuid);
                alarm.Principal = Principal;
                alarm.ProcessContent = ProcessContent;
                alarm.IsSuccess = IsSuccess;
                alarm.IsProcess = true;
                alarm.ProcessTime = DateTime.Now;
                alarm.Remark = Remark;
                await this.repository.PutAsync(alarm);
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
    }
}
