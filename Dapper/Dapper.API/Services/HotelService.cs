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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public async Task<Response<IEnumerable<Hotel>>> CreateManyAsync(IEnumerable<AddEditHotel> hotels, CancellationToken cancellationToken)
        {
            if(hotels is null || !hotels.Any())
            {
                return Response<IEnumerable<Hotel>>.Failure(SystemCodeEnum.NoHotelsToCreate);
            }

            var validationErrors = new List<Response<Hotel>>();


            foreach(var hotel in hotels)
            {
                HotelValidator validator = new HotelValidator();

                var result = await _helperFunctions.ProcessValidation<AddEditHotel, Hotel>(validator, hotel, new Response<Hotel>());

                if (!result.IsSuccess)
                {
                    validationErrors.Add(result);
                }
            }

            if (validationErrors.Any())
            {
                return Response<IEnumerable<Hotel>>.Failure(SystemCodeEnum.HotelCreationFailed);
            }

            var createdHotels = await _hotelRepository.CreateManyAsync(hotels.Select(h => _mapper.Map<HotelEntity>(h)), cancellationToken);

            return Response<IEnumerable<Hotel>>.Success(createdHotels.Select(h => _mapper.Map<Hotel>(h)));
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public async Task<Response<BulkDeleteResult>> DeleteManyAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting {Count} hotels", ids.Count());

            var currentUserId = 0;

            // Delete all hotels in a transaction
            var result = await _hotelRepository.DeleteManyAsync(ids, currentUserId, cancellationToken);

            _logger.LogInformation("Successfully processed bulk delete: {SuccessCount} deleted, {NotFoundCount} not found",
           result.SuccessfullyDeletedIds.Count(), result.NotFoundIds.Count());

            return Response<BulkDeleteResult>.Success(result);
        }

        ///<inheritdoc/>
        public async Task<Response<PaginatedResult<Hotel>>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default)
        {
            var hotels = await _hotelRepository.GetAll(pagination, cancellationToken);

            return Response<PaginatedResult<Hotel>>.Success(hotels);
        }

        ///<inheritdoc/>
        public async Task<Response<IEnumerable<Hotel>>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var hotels = await _hotelRepository.GetByIdsAsync(ids.ToArray(),cancellationToken);

            return Response<IEnumerable<Hotel>>.Success(hotels);
        }

        ///<inheritdoc/>
        public async Task<Response<Hotel>> GetHotelById(int id)
        {
            var hotel = await _hotelRepository.GetHotelById(id);

            if (hotel == null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelNotFound);
            }

            return Response<Hotel>.Success(hotel);
        }


        ///<inheritdoc/>
        public async Task<Response<Hotel>> GetHotelByName(string name)
        {
            var hotel = await _hotelRepository.GetHotelByName(name);

            if (hotel is null)
            {
                return Response<Hotel>.Failure(SystemCodeEnum.HotelNotFound);
            }

            return Response<Hotel>.Success(hotel);
        }

        ///<inheritdoc/>
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
