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
        /// Gets paginated list of hotels
        /// </summary>
        /// <param name="page">Current page (1-based indexing)</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>Paginated result containing hotels and metadata</returns>
        Task<Response<PaginatedResult<Hotel>>> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a hotel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<bool>> DeleteHotel(int id);

        /// <summary>
        /// Get a hotel by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<Hotel>> GetHotelById(int id);

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
