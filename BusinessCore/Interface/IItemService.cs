using Infrastructure;
using Shared;
namespace BusinessCore.Interface
{
	public partial interface IItemService
	{
		Task<(bool success, string message)> AddAsync(ItemDTO entity);
		Task<(bool success, string message)> UpdateAsync(ItemDTO entity);
		Task<(bool success, string message)> DeleteAsync(string Id);
		Task<Item> GetByIdAsync(string Id);
	}
}
