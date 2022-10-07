using Api.Controllers.Base;
using Api.Error;
using Api.Helper;
using AutoMapper;
using BusinessCore;
using BusinessCore.Helper;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;
namespace Api
{
	public partial class UnitMeasureController : BaseApiController
	{
		private readonly IRepository repository;
		private readonly IMapper mapper;
		public UnitMeasureController(IRepository repository, IMapper mapper) : base(repository, mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.mapper = mapper;
			this.mapper = mapper;
		}

		// GET: api/UnitMeasure
		[HttpGet]
		public async Task<IActionResult> ListAsync()
		{
			try
			{
				var items = await repository.GetQueryable<UnitMeasure>().AsNoTracking().ToListAsync();
				var result = mapper.Map<List<UnitMeasureDTO>>(items);
				return Requests.Response(this, new ApiStatus(200), result, Constant.Message.Success);
				}
				catch (Exception ex)
				{
					return Requests.Response(this, new ApiStatus(500), null, ex.Message);
				}
			}
		}
}
