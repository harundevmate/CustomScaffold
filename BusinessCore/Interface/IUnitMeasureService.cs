using Infrastructure;
using Shared;
namespace BusinessCore.Interface
{
	public partial interface IUnitMeasureService
	{
		Task<(bool success, string message)> AddAsync(UnitMeasureDTO entity);
		Task<(bool success, string message)> UpdateAsync(UnitMeasureDTO entity);
		Task<(bool success, string message)> DeleteAsync(string Id);
		Task<UnitMeasure> GetByIdAsync(string Id);
	}
}
