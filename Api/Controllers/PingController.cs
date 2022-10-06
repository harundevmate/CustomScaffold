using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Api.Controllers
{
    [Route("api/ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return StatusCode(200, "Success");
        }
        //// GET: api/ping
        //[HttpGet]
        //public IActionResult Ping()
        //{
        //    string isDebug = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();

        //    string info = "\n";
        //    string r = "PONG " + isDebug.ToUpper();

        //    try
        //    {
        //        if (isDebug != "production")
        //        {
        //            r = "PONG " + isDebug.ToUpper() + " MODE\n";

        //            var tExp = Helpers.Settings.AppSettingValue("AppSettings", "TokenExpired");
        //            var rtExp = Helpers.Settings.AppSettingValue("AppSettings", "RefreshTokenExpired");


        //            DateTime _datetime = DateTime.Now;
        //            DateTime _datetimeZoned = Helpers.Others.DateTimeConvertToZone(DateTime.Now);

        //            info = "Server Time " + _datetime.ToLongDateString() + " " + _datetime.ToLongTimeString() + "\n";
        //            info += "DateTimeConvertToZone Time " + _datetimeZoned.ToLongDateString() + " " + _datetimeZoned.ToLongTimeString() + "\n";
        //            info += "Token Expire : " + tExp + " Minutes\n";
        //            info += "Refresh Token Expire : " + rtExp + " Minutes\n";
        //            info += "Fitur Id :" + GetFiturId() + "\n";
        //            info += "Fitur Name :" + GetFiturName() + "\n";
        //            info += "\n";

        //            Assembly assembly = Assembly.GetExecutingAssembly();
        //            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        //            string ProdVersion = fileVersionInfo.ProductVersion;

        //            info += "API Version " + ProdVersion + "\n";

        //        }

        //        return StatusCode(200, r + info);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, r + info + "\n" + ex.Message + "\n" + ex.InnerException.Message.ToString());

        //    }

        //}
    }
}
