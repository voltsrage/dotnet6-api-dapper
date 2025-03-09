using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;
using Dapper.API.Models.Pagination;

namespace Dapper.API.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository Interface for the hotels
    /// </summary>
    public interface IHotelRepository
    {
        /// <summary>
        /// Add a new hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> AddHotel(HotelEntity model);

        /// <summary>
        /// Gets paginated list of hotels
        /// </summary>
        /// <param name="page">Current page (1-based indexing)</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>Paginated result containing hotels and metadata</returns>
        Task<PaginatedResult<Hotel>> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a hotel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteHotel(int id);

        /// <summary>
        /// Get a hotel by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Hotel> GetHotelById(int id);

        /// <summary>
        /// Get a hotel by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Hotel> GetHotelByName(string name);

        /// <summary>
        /// Get a hotel by it's name and address for uniqueness
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<Hotel> GetHotelByNameAndAddress(string name, string address);

        /// <summary>
        /// Update a hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateHotel(HotelEntity model);
    }
}
