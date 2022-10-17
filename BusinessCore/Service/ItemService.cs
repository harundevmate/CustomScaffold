using AutoMapper;
using BusinessCore.Interface;
using Infrastructure;
using Shared;
using Infrastructure.Interfaces;
namespace BusinessCore.Service
{
	public partial class ItemService : IItemService
	{
		private readonly IRepository repository;
		private readonly IMapper mapper;
		public ItemService(IRepository repository,IMapper mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
		}
		public async Task<(bool success, string message)> AddAsync(ItemDTO entity)
		{
			var item = mapper.Map<Item>(entity);
			item.CreatedAt = DateTime.Now;
			return await repository.AddAsync<Item>(item);
		}
		public async Task<(bool success, string message)> DeleteAsync(string Id)
		{
			return await repository.DeleteAsync<Item>(Id);
		}
		public async Task<Item> GetByIdAsync(string Id)
		{
			return await repository.GetByIdAsync<Item>(Id);
		}
		public async Task<(bool success, string message)> UpdateAsync(ItemDTO entity)
		{
			var item = mapper.Map<Item>(entity);
			item.ModifiedAt = DateTime.Now;
			return await repository.UpdateAsync<Item>(item);
		}
	}
}
