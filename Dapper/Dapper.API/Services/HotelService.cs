using AutoMapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;
using Dapper.API.Enums;
using Dapper.API.Helpers;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;
using Dapper.API.Services.Interfaces;
using Dapper.API.Validators;

namespace Dapper.API.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelService> _logger;
        private readonly IHelperFunctions _helperFunctions;

        public HotelService(
            ILogger<HotelService> logger,
            IMapper mapper,
            IHotelRepository hotelRepository,
            IHelperFunctions helperFunctions)
        {
            _logger = logger;
            _mapper = mapper;
            _hotelRepository = hotelRepository;
            _helperFunctions = helperFunctions;
        }

        public async Task<Response<Hotel>> AddHotel(AddEditHotel model)
        {
            var result = new Response<Hotel>();

            HotelValidator validator = new HotelValidator();
            result =  await _helperFunctions.ProcessValidation<AddEditHotel,Hotel>(validator,model,result);

            if (!result.IsSuccess)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodeEnum.BadRequest.Value;
                return result;
            }

            var existingHotel = await _hotelRepository.GetHotelByNameAndAddress(model.Name, model.Address);

            if (existingHotel != null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelAlreadyExists);
            }

            var hotelToSave = _mapper.Map<HotelEntity>(model);

            var hotelId = await _hotelRepository.AddHotel(hotelToSave);

            if (hotelId == 0)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelCreationFailed);
            }

            var hotel = await _hotelRepository.GetHotelById(hotelId);

            if(hotel is null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelNotFound);
            }

            return Response<Hotel>.Success(hotel);
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

        public async Task<Response<PaginatedResult<Hotel>>> GetAll(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var hotels = await _hotelRepository.GetAll(page, pageSize, searchTerm, cancellationToken);

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

            if (hotel is null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelNotFound);
            }

            return Response<Hotel>.Success(hotel);
        }

        public async Task<Response<bool>> UpdateHotel(int hotelId, AddEditHotel model)
        {
            var result = new Response<bool>();

            var existingHotel = await _hotelRepository.GetHotelById(hotelId);

            if (existingHotel == null)
            {
                return Response<bool>.Failure(SystemCodeEnum.HotelNotFound);
            }

            HotelValidator validator = new HotelValidator();
            result = await _helperFunctions.ProcessValidation<AddEditHotel, bool>(validator, model, result);

            if (!result.IsSuccess)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodeEnum.BadRequest.Value;
                return result;
            }

            var hotelToUpdate = _mapper.Map<HotelEntity>(model);
            hotelToUpdate.Id = hotelId;

            var updateResult = await _hotelRepository.UpdateHotel(hotelToUpdate);

            return Response<bool>.Success(updateResult);
        }
    }
}
