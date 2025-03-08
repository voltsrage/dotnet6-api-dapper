using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;

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
        /// Get all the hotels
        /// </summary>
        /// <returns></returns>
        Task<List<Hotel>> GetAll();

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
        /// Update a hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateHotel(HotelEntity model);
    }
}
