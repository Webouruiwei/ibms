using BdlIBMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BdlIBMS.Controllers
{
    public class SettingsController : ApiController
    {
        private const string ConnectName = "IbmsContext";

        [Route("api/settings/dbconnect")]
        [HttpGet]
        public IHttpActionResult GetDbConnection()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string connectString = WebConfigHelper.ReadConnectString(ConnectName);
            Dictionary<string, string> dict = WebConfigHelper.ResolveConnectString(connectString);
            var item = new
            {
                ServiceName = dict.ContainsKey("data source") ? dict["data source"] : Dns.GetHostName(),
                DbName = dict.ContainsKey("initial catalog") ? dict["initial catalog"] : "BdlIBMS",
                UserName = dict.ContainsKey("user id") ? dict["user id"] : "sa",
                Password = dict["password"]
            };

            return Ok(item);
        }

        [Route("api/settings/testconnect")]
        [HttpGet]
        [ResponseType(typeof(Boolean))]
        public IHttpActionResult TestDbConnection()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string ServiceName = HttpContext.Current.Request.Params["ServiceName"];
            string DbName = HttpContext.Current.Request.Params["DbName"];
            string UserName = HttpContext.Current.Request.Params["UserName"];
            string Password = HttpContext.Current.Request.Params["Password"];
            string ConnectString = string.Format("data source={0};initial catalog={1};persist security info=True;user id={2};password={3};" +
            "MultipleActiveResultSets=True;App=EntityFramewor", ServiceName, DbName, UserName, Password);
            bool isSuccess = WebConfigHelper.TestDbConnect(ConnectString);

            return Ok(isSuccess);
        }

        [Route("api/settings/dbconnect")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDbConnection()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string ServiceName = HttpContext.Current.Request.Params["ServiceName"];
            string DbName = HttpContext.Current.Request.Params["DbName"];
            string UserName = HttpContext.Current.Request.Params["UserName"];
            string Password = HttpContext.Current.Request.Params["Password"];
            string ConnectString = string.Format("data source={0};initial catalog={1};persist security info=True;user id={2};password={3};" +
            "MultipleActiveResultSets=True;App=EntityFramewor", ServiceName, DbName, UserName, Password);
            WebConfigHelper.WriteConnectString(ConnectName, ConnectString);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/settings/system")]
        [HttpGet]
        public IHttpActionResult GetSystemSetting()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            bool IsRefresh = Convert.ToBoolean(WebConfigHelper.ReadAppSetting("IsRefresh"));
            bool IsDraggable = Convert.ToBoolean(WebConfigHelper.ReadAppSetting("IsDraggable"));
            var item = new
            {
                IsRefresh = IsRefresh,
                IsDraggable = IsDraggable
            };

            return Ok(item);
        }

        [Route("api/settings/system")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSystemSetting()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string IsRefresh = HttpContext.Current.Request.Params["IsRefresh"];
            string IsDraggable = HttpContext.Current.Request.Params["IsDraggable"];
            WebConfigHelper.WriteAppSetting("IsRefresh", IsRefresh);
            WebConfigHelper.WriteAppSetting("IsDraggable", IsDraggable);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
