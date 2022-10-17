using Api.Error;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
namespace Api.Helper
{


    public class ApiResult
    {
        public string status { get; set; }
        public string error { get; set; }
        public string detail { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class Requests
    {
        internal static IActionResult Response(ControllerBase Controller, ApiStatus statusCode)
        {
            return Response(Controller, statusCode, null, "");
        }
        internal static IActionResult ResponseToken(ControllerBase Controller, ApiStatus statusCode, string token)
        {
            return Response(Controller, statusCode, new { token = token }, "");
        }
        internal static IActionResult DataTableResponse(ControllerBase Controller, ApiStatus statusCode, int _recordsTotal, object dataValue)
        {
            var ResultDataValue = new
            {
                draw = 0,
                recordsFiltered = _recordsTotal,
                recordsTotal = _recordsTotal,
                data = dataValue
            };

            return Controller.StatusCode(statusCode.StatusCode, ResultDataValue);
        }

        internal static IActionResult Response(ControllerBase Controller, ApiStatus statusCode, object dataValue, string msg)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = "",
                message = e.StatusDescription,
                data = dataValue
            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue
                };
            }
            else
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = false,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue

                };
            }
            return Controller.StatusCode(statusCode.StatusCode, _);
        }

    }
}
