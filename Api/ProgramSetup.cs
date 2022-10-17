using BusinessCore.Interface;
using BusinessCore.Service;
namespace Api
{
	public partial class ProgramSetup
	{
		public static void AddDepedencyInjectionService(IServiceCollection services)
		{
			services.AddTransient<IItemService, ItemService>();
			services.AddTransient<IUnitMeasureService, UnitMeasureService>();
		}
	}
}
