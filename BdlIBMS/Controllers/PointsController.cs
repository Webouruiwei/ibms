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
    public class PointsController : ApiController
    {
        IPointRepository pointRepository;
        IRepository<int, OperationRecord> operationRecordRepository;

        public PointsController(IPointRepository pointRepository,
                                IRepository<int, OperationRecord> operationRecordRepository)
        {
            this.pointRepository = pointRepository;
            this.operationRecordRepository = operationRecordRepository;
        }

        // GET: api/points
        public IHttpActionResult GetPoints()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Point> points;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                points = this.pointRepository.GetOriginalAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.pointRepository.GetOriginalCount());
                points = this.pointRepository.GetPagerItems(pageIndex, pageSize, u => u.PointID);
            }

            var items = from item in points
                        select new
                        {
                            ID = item.ID,
                            PointID = item.PointID,
                            ModuleID = item.ModuleID,
                            ModuleName = item.Module.Name,
                            Protocol = item.Protocol,
                            AreaID = item.AreaID,
                            AreaName = item.Area.Name,
                            Floor = item.Floor,
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDescription,
                            ValueFunc = item.ValueFunc,
                            MinValue = item.MinValue,
                            MaxValue = item.MaxValue,
                            Type = item.Type,
                            Unit = item.Unit,
                            IsArchive = item.IsArchive,
                            ArchiveInterval = item.ArchiveInterval,
                            ParentID = item.ParentID
                        };
            pager.Items = items;

            return Ok(pager);
        }

        [Route("api/points/factors")]
        [HttpGet]
        public IHttpActionResult GetPointsByFactors()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string ModuleID = HttpContext.Current.Request.Params["ModuleID"];
            int AreaID = Convert.ToInt32(HttpContext.Current.Request.Params["AreaID"]);
            string Floor = HttpContext.Current.Request.Params["Floor"];
            IEnumerable<Point> points = this.pointRepository.GetAll(ModuleID, AreaID, Floor);
            var items = from item in points
                        select new
                        {
                            ID = item.ID,
                            PointID = item.PointID,
                            ModuleID = item.ModuleID,
                            ModuleName = item.Module.Name,
                            Protocol = item.Protocol,
                            AreaID = item.AreaID,
                            AreaName = item.Area.Name,
                            Floor = item.Floor,
                            TopPos = item.TopPos,
                            LeftPos = item.LeftPos,
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDescription,
                            ValueFunc = item.ValueFunc,
                            Value = item.Value,
                            MinValue = item.MinValue,
                            MaxValue = item.MaxValue,
                            Type = item.Type,
                            Unit = item.Unit,
                            DateTime = item.DateTime,
                            IsArchive = item.IsArchive,
                            ArchiveInterval = item.ArchiveInterval,
                            ParentID = item.ParentID
                        };

            return Ok(items);
        }

        [Route("api/points/trend")]
        [HttpGet]
        public IHttpActionResult GetPointsTrend()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string PointIDs = HttpContext.Current.Request.Params["PointIDs"];
            DateTime StartTime = DateTime.ParseExact(HttpContext.Current.Request.Params["StartTime"], "yyyy-MM-dd HH", null);
            DateTime EndTime = DateTime.ParseExact(HttpContext.Current.Request.Params["EndTime"], "yyyy-MM-dd HH", null);
            string[] pointAry = PointIDs.Split(',');
            bool first = true;
            var timelines = new List<string>();
            var valuelines = new List<decimal>[pointAry.Length];
            for (int i = 0; i < pointAry.Length; i++)
            {
                valuelines[i] = new List<decimal>();
                string PointID = pointAry[i];
                IEnumerable<TrendData> datas = this.pointRepository.GetTrendData(PointID, StartTime, EndTime);
                foreach (TrendData data in datas)
                {
                    if (first)
                        timelines.Add(data.Timeline);
                    valuelines[i].Add(data.Valueline);
                }
                first = false;
            }
            
            var items = new
            {
                Timelines = timelines,
                Valuelines = valuelines
            };

            return Ok(items);
        }

        // GET: api/points/400001
        [ResponseType(typeof(Point))]
        public async Task<IHttpActionResult> GetPoint(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Point item = await this.pointRepository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                PointID = item.PointID,
                ModuleID = item.ModuleID,
                ModuleName = item.Module.Name,
                Protocol = item.Protocol,
                AreaID = item.AreaID,
                AreaName = item.Area.Name,
                Floor = item.Floor,
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                ItemDescription = item.ItemDescription,
                ValueFunc = item.ValueFunc,
                MinValue = item.MinValue,
                MaxValue = item.MaxValue,
                Type = item.Type,
                Unit = item.Unit,
                IsArchive = item.IsArchive,
                ArchiveInterval = item.ArchiveInterval,
                ParentID = item.ParentID
            };

            return Ok(result);
        }

        // PUT: api/points/400001
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPoint(int uuid, [FromUri]Point point)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != point.ID)
                return BadRequest();

            try
            {
                await this.pointRepository.PutAsync(point);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.pointRepository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/points/position/{uuid}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutPointPostion(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (!this.pointRepository.IsExist(uuid))
                return NotFound();

            double Left = Convert.ToDouble(HttpContext.Current.Request.Params["Left"]);
            double Top = Convert.ToDouble(HttpContext.Current.Request.Params["Top"]);
            Point point = await this.pointRepository.GetByIdAsync(uuid);
            point.LeftPos = Left;
            point.TopPos = Top;
            try
            {
                await this.pointRepository.PutAsync(point);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/points/write")]
        [HttpPut]
        public async Task<IHttpActionResult> PutPointWriteValue()
        {
            bool result = false;
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            UserSession session = HttpContext.Current.Session["mySession"] as UserSession;
            if (session == null)
                return NotFound();

            string PointID = HttpContext.Current.Request.Params["PointID"];
            string Value = HttpContext.Current.Request.Params["Value"];
            OperationRecord operationRecord = new OperationRecord();
            operationRecord.UserName = session.UserName;
            operationRecord.IP = TextHelper.GetHostAddress();
            operationRecord.DateTime = DateTime.Now;
            operationRecord.Content = string.Format("点位ID为{0}，写入值为{1}", PointID, Value);
            Point point = this.pointRepository.GetByPointID(PointID);
            if (point == null)
            {
                operationRecord.Result = string.Format("ID为{0}的点位不存在，写入失败！", PointID);
                await this.operationRecordRepository.AddAsync(operationRecord);
                return NotFound();
            }

            string ItemID = point.ItemID;
            try
            {
                using (var proxy = new BdlIBMS.WriterService.WriterServiceClient())
                {
                    switch (point.Protocol)
                    {
                        case "OPC":
                            result = proxy.OpcWrite(ItemID, Value);
                            break;
                        case "MODBUS":
                            byte funcode = Convert.ToByte(Convert.ToInt32(ItemID[0].ToString()) - 1);
                            ushort adr = Convert.ToUInt16(Convert.ToInt32(ItemID.Substring(1)) - 1);
                            ushort usvalue = Convert.ToUInt16(Value);
                            result = proxy.ModbusWrite(adr, usvalue, funcode, 1);
                            break;
                        case "BACNET":
                            uint uiitemID = Convert.ToUInt32(ItemID);
                            uint uivalue = Convert.ToUInt32(Value);
                            result = proxy.BacnetWrite(uiitemID, uivalue);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                operationRecord.Content += ",错误内容为:" + ex.Message;
            }

            // 如果写入成功，则把最新设置的值更新到数据库
            if (result)
            {
                try
                {
                    point.Value = Value;
                    await this.pointRepository.PutAsync(point);
                    operationRecord.Result = string.Format("ID为{0},ItemID为{1},协议为{2}的点位写入成功！", PointID, ItemID, point.Protocol);
                    await this.operationRecordRepository.AddAsync(operationRecord);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                operationRecord.Result = string.Format("ID为{0},ItemID为{1},协议为{2}的点位写入失败！", PointID, ItemID, point.Protocol);
                await this.operationRecordRepository.AddAsync(operationRecord);
            }

            return Ok(result);
        }

        // POST: api/points
        [ResponseType(typeof(Point))]
        public async Task<IHttpActionResult> PostPoint([FromUri]Point point)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                point.TopPos = 0;
                point.LeftPos = 0;
                point.Status = false;
                point.DateTime = DateTime.Now;
                point.ArchiveTime = DateTime.Now;
                point.ArchiveTag = false;
                await this.pointRepository.AddAsync(point);
            }
            catch (DbUpdateException)
            {
                if (this.pointRepository.IsExist(point.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/points/400001
        public async Task<IHttpActionResult> DeletePoint(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Point point = await this.pointRepository.GetByIdAsync(uuid);
            if (point == null)
                return NotFound();

            await this.pointRepository.DeleteAsync(point);

            return Ok();
        }
    }
}
