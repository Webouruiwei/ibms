using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BdlIBMS.Utils
{
    public class TextHelper
    {
        /// <summary>
        /// 生成一个32位的UUID
        /// </summary>
        /// <returns></returns>
        public static string GenerateUUID()
        {
            string uuid = Guid.NewGuid().ToString();
            uuid = uuid.Replace("-", "");
            return uuid;
        }

        ///   <summary>  
        ///   给一个字符串进行MD5加密  
        ///   </summary>  
        ///   <param   name="strText">待加密字符串</param>  
        ///   <returns>加密后的字符串</returns>  
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strText);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        /// <summary>
        /// 判断用户是否登录/签到
        /// </summary>
        /// <returns></returns>
        public static bool IsSignin()
        {
            var userInfo = HttpContext.Current.Session["mySession"] as BdlIBMS.Models.UserSession;
            return userInfo == null ? false : true;
        }

        /// <summary>
        /// 检验用户是否已经授权访问系统
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static System.Web.Http.IHttpActionResult CheckAuthorized(System.Net.Http.HttpRequestMessage request)
        {
            if (!IsSignin())
            {
                var errorResult = new BdlIBMS.Results.InternalServerErrorTextPlainResult("未授权访问，请先登录！", request);
                errorResult.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return errorResult;
            }

            return null;
        }
    }
}