using Dapper.API.Dtos.Rooms;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;

namespace Dapper.API.Services.Interfaces
{
    /// <summary>
    /// Interface for room management operations
    /// </summary>
    public interface IRoomService
    {
        /// <summary>
        /// Gets a room by its ID
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The room if found; otherwise null</returns>
        Task<Response<Room>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets rooms by hotel ID with pagination and filtering
        /// </summary>
        /// <param name="hotelId">The hotel ID</param>
        /// <param name="request">Pagination and filtering parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of rooms</returns>
        Task<Response<PaginatedResult<Room>>> GetByHotelIdAsync(int hotelId, PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all rooms with pagination and filtering
        /// </summary>
        /// <param name="request">Pagination and filtering parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of rooms</returns>
        Task<Response<PaginatedResult<Room>>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new room
        /// </summary>
        /// <param name="room">The room to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created room with ID assigned</returns>
        Task<Response<Room>> CreateAsync(AddEditRoom room, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing room
        /// </summary>
        /// <param name="id">The room ID to update</param>
        /// <param name="room">Updated room data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated room</returns>
        Task<Response<Room>> UpdateAsync(int id, AddEditRoom room, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a room
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if deleted; otherwise false</returns>
        Task<Response<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the availability status of a room
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="isAvailable">New availability status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if updated; otherwise false</returns>
        Task<Response<bool>> UpdateAvailabilityAsync(int id, bool isAvailable, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all room types
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of room types</returns>
        Task<Response<IEnumerable<RoomType>>> GetRoomTypesAsync(CancellationToken cancellationToken = default);
    }
}
