using Dapper.API.Dtos.Rooms;
using Dapper.API.Entities;
using Dapper.API.Models.Pagination;

namespace Dapper.API.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interface for room data access operations
    /// </summary>
    public interface IRoomRepository
    {
        /// <summary>
        /// Gets a room by its ID
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The room if found; otherwise null</returns>
        Task<Room> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets rooms by hotel ID with optional filtering
        /// </summary>
        /// <param name="hotelId">The hotel ID</param>
        /// <param name="request">Pagination and filtering parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of rooms</returns>
        Task<PaginatedResult<Room>> GetByHotelIdAsync(int hotelId, PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all rooms with pagination and filtering
        /// </summary>
        /// <param name="request">Pagination and filtering parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of rooms</returns>
        Task<PaginatedResult<Room>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new room
        /// </summary>
        /// <param name="room">The room to create</param>
        /// <param name="userId">ID of the user creating the room</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created room with ID assigned</returns>
        Task<Room> CreateAsync(RoomEntity room, int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing room
        /// </summary>
        /// <param name="id">The room ID to update</param>
        /// <param name="room">Updated room data</param>
        /// <param name="userId">ID of the user updating the room</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated room</returns>
        Task<Room> UpdateAsync(RoomEntity room, int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a room by setting its status to deleted
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="userId">ID of the user deleting the room</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if deleted; otherwise false</returns>
        Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the availability status of a room
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="isAvailable">New availability status</param>
        /// <param name="userId">ID of the user updating the status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if updated; otherwise false</returns>
        Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable, int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all room types
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of room types</returns>
        Task<IEnumerable<RoomType>> GetRoomTypesAsync(CancellationToken cancellationToken = default);
    }
}
