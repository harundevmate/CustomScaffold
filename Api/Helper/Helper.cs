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

        public static IActionResult ResponseCUSTOMBANK<T>(ControllerBase Controller, ApiStatus statusCode, object dataValue, string msg, List<T> Obj)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = "",
                message = e.StatusDescription,
                data = dataValue,
                Obj = Obj

            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue,
                    Obj = Obj
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
                    data = dataValue,
                    Obj = Obj

                };
            }


            return Controller.StatusCode(statusCode.StatusCode, _);
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

        internal static IActionResult ResponseDetailVerifikasiDatatable(ControllerBase Controller, ApiStatus statusCode, object dataValueHeader, object PerencanaanVerifPremiDetail, string msg)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = "",
                message = e.StatusDescription,
                data = dataValueHeader,
                PerencanaanVerifPremiDetail
            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValueHeader,
                    PerencanaanVerifPremiDetail
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
                    data = dataValueHeader,
                    PerencanaanVerifPremiDetail
                };
            }


            return Controller.StatusCode(statusCode.StatusCode, _);
        }

        /// <summary>
        /// [GPRM] UC-01-N-002
        /// Memberikan notifikasi jika ada bank yang kurang / lebih bayar premi
        /// </summary>
        /// <returns></returns>
        #region [GPRM] UC-01-N-002
        internal static IActionResult ResponseDatatableDendaPremi(ControllerBase Controller, ApiStatus statusCode, object dataValue, string msg, string detail)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = e.StatusDescription,
                message = "",
                data = dataValue
            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = statusCode.StatusDescription + " ! Total " + detail + " Record Data",
                    message = msg,
                    data = dataValue
                };
            }
            else
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = false,
                    detail = statusCode.StatusDescription + " ! Total " + detail + " Record Data",
                    message = msg,
                    data = dataValue

                };
            }


            return Controller.StatusCode(statusCode.StatusCode, _);
        }
        #endregion

        /// <summary> Created By : Mochammad Reza Fahlevi
        /// [GPRM] UC-01-V-005
        /// Melihat/Mengunduh Report Data Terkait Hasil Verifikasi Premi
        /// </summary>
        /// <Author>
        /// Created By : Mochammad Reza Fahlevi
        /// </Author>
        /// <returns>
        /// POST : api/VerifikasiPremiHeader/LaporanHasilVerifikasiPremi/GridList
        /// </returns>
        #region [GPRM] UC-01-V-005 : Created By Mochammad Reza Fahlevi
        internal static IActionResult ResponseDatatableLaporanHasilVerifikasiPremi(ControllerBase Controller, ApiStatus statusCode, object dataValue, string msg, string detail)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = e.StatusDescription,
                message = "",
                data = dataValue
            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = statusCode.StatusDescription + " ! Total " + detail + " Record Data",
                    message = msg,
                    data = dataValue
                };
            }
            else
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = false,
                    detail = statusCode.StatusDescription + " ! Total " + detail + " Record Data",
                    message = msg,
                    data = dataValue

                };
            }


            return Controller.StatusCode(statusCode.StatusCode, _);
        }
        #endregion
    }
}
