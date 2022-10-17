using AutoMapper;
using BusinessCore.Interface;
using Infrastructure;
using Shared;
using Infrastructure.Interfaces;
namespace BusinessCore.Service
{
	public partial class UnitMeasureService : IUnitMeasureService
	{
		private readonly IRepository repository;
		private readonly IMapper mapper;
		public UnitMeasureService(IRepository repository,IMapper mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
		}
		public async Task<(bool success, string message)> AddAsync(UnitMeasureDTO entity)
		{
			var item = mapper.Map<UnitMeasure>(entity);
			item.CreatedAt = DateTime.Now;
			return await repository.AddAsync<UnitMeasure>(item);
		}
		public async Task<(bool success, string message)> DeleteAsync(string Id)
		{
			return await repository.DeleteAsync<UnitMeasure>(Id);
		}
		public async Task<UnitMeasure> GetByIdAsync(string Id)
		{
			return await repository.GetByIdAsync<UnitMeasure>(Id);
		}
		public async Task<(bool success, string message)> UpdateAsync(UnitMeasureDTO entity)
		{
			var item = mapper.Map<UnitMeasure>(entity);
			item.ModifiedAt = DateTime.Now;
			return await repository.UpdateAsync<UnitMeasure>(item);
		}
	}
}
