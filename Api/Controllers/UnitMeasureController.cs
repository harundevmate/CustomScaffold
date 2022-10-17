using Api.Controllers.Base;
using Api.Helper;
using BusinessCore.Interface;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Helper;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitMeasureController : BaseApiController
    {
        private readonly IUnitMeasureService unitMeasureService;
        public UnitMeasureController(IUnitMeasureService unitMeasureService)
        {
            this.unitMeasureService = unitMeasureService;
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetById(string Id)
        {
            var r = await unitMeasureService.GetByIdAsync(Id);
            return Requests.Response(this, new Error.ApiStatus(200), r, Constant.Message.Success);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] UnitMeasureDTO item)
        {
            if(this.ModelState.IsValid) throw new InvalidOperationException(Constant.Exception.InvalidModelState);
            var r = await unitMeasureService.AddAsync(item);
            return Requests.Response(this, new Error.ApiStatus(r.success ? 200 : 409), null, r.message);
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateAsync([FromBody] UnitMeasureDTO item)
        {
            if (this.ModelState.IsValid) throw new InvalidOperationException(Constant.Exception.InvalidModelState);
            var r = await unitMeasureService.UpdateAsync(item);
            return Requests.Response(this, new Error.ApiStatus(r.success ? 200 : 409), null, r.message);
        }
        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteAsync(string Id)
        {
            var r = await unitMeasureService.DeleteAsync(Id);
            return Requests.Response(this, new Error.ApiStatus(r.success ? 200 : 409), null, r.message);
        }
    }
}
