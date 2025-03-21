using Dapper.API.Entities.Amentities;

namespace Dapper.API.Data.Repositories.Interfaces.Amenities
{
    /// <summary>
    /// Interface for Room Type Amenity Repository
    /// </summary>
    public interface IRoomTypeAmenityRepository
    {
        /// <summary>
        /// Get all room type amenities
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeIdAsync(int roomTypeId);

        /// <summary>
        /// Get all amenities for room type
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        Task<IEnumerable<BaseAmenity>> GetAmenitiesForRoomTypeAsync(int roomTypeId);

        /// <summary>
        /// Get all default amenities for room type
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        Task<IEnumerable<BaseAmenity>> GetDefaultAmenitiesForRoomTypeAsync(int roomTypeId);

        /// <summary>
        /// Get room type amenity by room type and amenity id
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<RoomTypeAmenity> GetByRoomTypeAndAmenityIdAsync(int roomTypeId, int amenityId);

        /// <summary>
        /// Add room type amenity
        /// </summary>
        /// <param name="roomTypeAmenity"></param>
        /// <returns></returns>
        Task<int> AddRoomTypeAmenityAsync(RoomTypeAmenity roomTypeAmenity);

        /// <summary>
        /// Update room type amenity
        /// </summary>
        /// <param name="roomTypeAmenity"></param>
        /// <returns></returns>
        Task UpdateRoomTypeAmenityAsync(RoomTypeAmenity roomTypeAmenity);

        /// <summary>
        /// Remove room type amenity
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task RemoveRoomTypeAmenityAsync(int roomTypeId, int amenityId);

        /// <summary>
        /// Set all room type amenities default status
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task SetAllRoomTypeAmenitiesDefaultStatusAsync(int roomTypeId, bool isDefault);
    }
}
