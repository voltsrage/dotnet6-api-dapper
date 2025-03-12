using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;

namespace Dapper.API.Services.Interfaces
{
    /// <summary>
    /// Hotel Service Interface
    /// </summary>
    public interface IHotelService
    {
        /// <summary>
        /// Add a new hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response<Hotel>> AddHotel(AddEditHotel model);

        /// <summary>
        /// Creates multiple hotels in a single transaction
        /// </summary>
        /// <param name="hotels">Collection of hotels to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created hotels with IDs assigned</returns>
        Task<Response<IEnumerable<Hotel>>> CreateManyAsync(
            IEnumerable<AddEditHotel> hotels,
            CancellationToken cancellationToken);

        /// <summary>
        /// Creates a hotel with its rooms
        /// </summary>
        /// <param name="hotelWithRooms">The hotel data with rooms</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created hotel with its rooms and IDs assigned</returns>
        Task<Response<HotelWithRooms>> CreateHotelWithRoomsAsync(AddHotelWithRooms hotelWithRooms, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets paginated list of hotels
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>Paginated result containing hotels and metadata</returns>
        Task<Response<PaginatedResult<Hotel>>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets hotels with their rooms with pagination and filtering
        /// </summary>
        /// <param name="request">Pagination and filtering parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of hotels with their rooms</returns>
        Task<Response<PaginatedResult<HotelWithRooms>>> GetHotelsWithRoomsAsync(PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a hotel with its rooms by hotel ID
        /// </summary>
        /// <param name="hotelId">The hotel ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The hotel with its rooms, or null if not found</returns>
        Task<Response<HotelWithRooms>> GetHotelWithRoomsByIdAsync(int hotelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a hotel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<bool>> DeleteHotel(int id);

        /// <summary>
        /// Deletes multiple hotels in a single transaction
        /// </summary>
        /// <param name="ids">Collection of hotel IDs to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the bulk delete operation</returns>
        Task<Response<BulkDeleteResult>> DeleteManyAsync(
            IEnumerable<int> ids,
            CancellationToken cancellationToken);

        /// <summary>
        /// Get a hotel by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<Hotel>> GetHotelById(int id);

        /// <summary>
        /// Gets hotels by their IDs
        /// </summary>
        /// <param name="ids">Collection of hotel IDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Found hotels</returns>
        Task<Response<IEnumerable<Hotel>>> GetByIdsAsync(
            IEnumerable<int> ids,
            CancellationToken cancellationToken);

        /// <summary>
        /// Get a hotel by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Response<Hotel>> GetHotelByName(string name);

        /// <summary>
        /// Update a hotel
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response<bool>> UpdateHotel(int hotelId, AddEditHotel model);
    }
}
