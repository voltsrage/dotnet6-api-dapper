using AutoMapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Rooms;
using Dapper.API.Entities;
using Dapper.API.Enums;
using Dapper.API.Helpers;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;
using Dapper.API.Services.Interfaces;
using Dapper.API.Validators;

namespace Dapper.API.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly IHelperFunctions _helperFunctions;

        public RoomService(
            IRoomRepository roomRepository,
            IMapper mapper,
            IHelperFunctions helperFunctions)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _helperFunctions = helperFunctions;
        }

        public async Task<Response<Room>> CreateAsync(AddEditRoom room, CancellationToken cancellationToken = default)
        {
            var result = new Response<Room>();

            RoomValidator validator = new RoomValidator();

            result = await _helperFunctions.ProcessValidation<AddEditRoom, Room>(validator, room, result);

            if (!result.IsSuccess)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodeEnum.BadRequest.Value;
                return result;
            }

            var roomToSave = _mapper.Map<RoomEntity>(room);

            var createdRoom = await _roomRepository.CreateAsync(roomToSave, 0, cancellationToken);

            if (createdRoom is null)
            {
                return Response<Room>.Failure(SystemCodeEnum.RoomCreationFailed);
            }

            return Response<Room>.Success(createdRoom);

        }

        public async Task<Response<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

            if (room is null)
            {
                return Response<bool>.Failure(SystemCodeEnum.RoomNotFound);
            }

            var userId = 0;

            var isDeleted = await _roomRepository.DeleteAsync(id,userId, cancellationToken);

            return Response<bool>.Success(isDeleted);
        }

        public async Task<Response<PaginatedResult<Room>>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var rooms = await _roomRepository.GetAllAsync(request, cancellationToken);

            return Response<PaginatedResult<Room>>.Success(rooms);
        }

        public async Task<Response<PaginatedResult<Room>>> GetByHotelIdAsync(int hotelId, PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var hotelRooms = await _roomRepository.GetByHotelIdAsync(hotelId, request, cancellationToken);

            return Response<PaginatedResult<Room>>.Success(hotelRooms);
        }

        public async Task<Response<Room>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var room =  await _roomRepository.GetByIdAsync(id, cancellationToken);

            if (room is null)
            {
                return Response<Room>.Failure(SystemCodeEnum.RoomNotFound);
            }

            return Response<Room>.Success(room);
        }

        public async Task<Response<IEnumerable<RoomType>>> GetRoomTypesAsync(CancellationToken cancellationToken = default)
        {
            var roomTypes = await _roomRepository.GetRoomTypesAsync(cancellationToken);

            return Response<IEnumerable<RoomType>>.Success(roomTypes);
        }

        public async Task<Response<Room>> UpdateAsync(int id, AddEditRoom room, CancellationToken cancellationToken = default)
        {
            var result = new Response<Room>();

            RoomValidator validator = new RoomValidator();

            result = await _helperFunctions.ProcessValidation<AddEditRoom, Room>(validator, room, result);

            if (!result.IsSuccess)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodeEnum.BadRequest.Value;
                return result;
            }
            
            var roomToUpdate = _mapper.Map<RoomEntity>(room);
            roomToUpdate.Id = id;

            var userId = 0;

            var updatedRoom = await _roomRepository.UpdateAsync(roomToUpdate,userId, cancellationToken);

            return Response<Room>.Success(updatedRoom);
        }

        public async Task<Response<bool>> UpdateAvailabilityAsync(int id, bool isAvailable, CancellationToken cancellationToken = default)
        {
            var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

            if (room is null)
            {
                return Response<bool>.Failure(SystemCodeEnum.RoomNotFound);
            }

            var isUpdated = await _roomRepository.UpdateAvailabilityAsync(id, isAvailable, 0, cancellationToken);

            return Response<bool>.Success(isUpdated);
        }
    }
}
