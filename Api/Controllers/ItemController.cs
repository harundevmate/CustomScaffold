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
	public partial class ItemController : BaseApiController
	{
		private readonly IRepository repository;
		private readonly IMapper mapper;
		public ItemController(IRepository repository, IMapper mapper) : base(repository, mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.mapper = mapper;
			this.mapper = mapper;
		}

		// GET: api/Item
		[HttpGet]
		public async Task<IActionResult> ListAsync()
		{
			try
			{
				var items = await repository.GetQueryable<Item>().AsNoTracking().ToListAsync();
				var result = mapper.Map<List<ItemDTO>>(items);
				return Requests.Response(this, new ApiStatus(200), result, Constant.Message.Success);
				}
				catch (Exception ex)
				{
					return Requests.Response(this, new ApiStatus(500), null, ex.Message);
				}
			}
		}
}