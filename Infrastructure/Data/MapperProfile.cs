using AutoMapper;
using BusinessCore;

namespace Infrastructure.Data
{
    public class MapperProfile : MapperConfigurationExpression
    {
        public MapperProfile()
        {
            CreateMap<Item, ItemDTO>();
        }
    }
}
