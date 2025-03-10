﻿using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;
using Dapper.API.Models;
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
        /// Creates multiple hotels in a single transaction
        /// </summary>
        /// <param name="hotels">Collection of hotels to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created hotels with IDs assigned</returns>
        Task<IEnumerable<HotelEntity>> CreateManyAsync(
            IEnumerable<HotelEntity> hotels,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets paginated list of hotels
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>Paginated result containing hotels and metadata</returns>
        Task<PaginatedResult<Hotel>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a hotel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteHotel(int id);

        /// <summary>
        /// Deletes multiple hotels in a single transaction
        /// </summary>
        /// <param name="ids">Collection of hotel IDs to delete</param>
        /// <param name="userId">ID of the user performing the delete operation</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the bulk delete operation</returns>
        Task<BulkDeleteResult> DeleteManyAsync(
            IEnumerable<int> ids,
            int userId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Get a hotel by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Hotel> GetHotelById(int id);

        /// <summary>
        /// Gets hotels by their IDs
        /// </summary>
        /// <param name="ids">Collection of hotel IDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Found hotels</returns>
        Task<IEnumerable<Hotel>> GetByIdsAsync(
            int[] ids,
            CancellationToken cancellationToken);

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
