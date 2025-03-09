using AutoMapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Enums;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;

namespace Dapper.API.Services.Interfaces
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelService> _logger;

        public HotelService(
            ILogger<HotelService> logger,
            IMapper mapper,
            IHotelRepository hotelRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _hotelRepository = hotelRepository;
        }

        public async Task<Response<int>> AddHotel(AddEditHotel model)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> DeleteHotel(int id)
        {
            var hotel = await _hotelRepository.GetHotelById(id);

            if (hotel == null)
            {
                return Response<bool>.Failure(SystemCodeEnum.HotelNotFound);
            }

            var result = await _hotelRepository.DeleteHotel(id);

            return Response<bool>.Success(result);
        }

        public async Task<Response<PaginatedResult<Hotel>>> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var hotels = await _hotelRepository.GetAll(page, pageSize, cancellationToken);

            return Response<PaginatedResult<Hotel>>.Success(hotels);
        }

        public async Task<Response<Hotel>> GetHotelById(int id)
        {
            var hotel = await _hotelRepository.GetHotelById(id);

            if (hotel == null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelNotFound);
            }

            return Response<Hotel>.Success(hotel);
        }

        public async Task<Response<Hotel>> GetHotelByName(string name)
        {
            var hotel = await _hotelRepository.GetHotelByName(name);

            if(hotel is null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelNotFound);
            }

            return Response<Hotel>.Success(hotel);
        }

        public async Task<Response<bool>> UpdateHotel(int hotelId, AddEditHotel model)
        {
            throw new NotImplementedException();
        }
    }
}
