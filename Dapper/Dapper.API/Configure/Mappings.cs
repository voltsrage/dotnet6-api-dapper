using AutoMapper;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;

namespace Dapper.API.Configure
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<HotelEntity, Hotel>()
                .ReverseMap();

            CreateMap<HotelEntity, AddEditHotel>()
                .ReverseMap();
        }
    }
}
