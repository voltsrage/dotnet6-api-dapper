using AutoMapper;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Dtos.Rooms;
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

            CreateMap<RoomEntity, Room>()
               .ReverseMap();

            CreateMap<RoomEntity, AddEditRoom>()
                .ReverseMap();
        }


    }
}
