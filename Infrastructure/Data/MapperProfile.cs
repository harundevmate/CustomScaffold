using AutoMapper;
using Shared;
namespace Infrastructure
{
	public class MapperProfile : MapperConfigurationExpression
	{
		public MapperProfile()
		{
			CreateMap<Item, ItemDTO>().ReverseMap();
			CreateMap<UnitMeasure, UnitMeasureDTO>().ReverseMap();
		}
	}
}
