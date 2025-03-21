using Dapper.API.Dtos.Rooms;
using Dapper.API.Entities.Amentities;
using Dapper.API.Models;

namespace Dapper.API.Services.Interfaces.Amenities
{
    /// <summary>
    /// Interface for Room Type Amenity Service
    /// </summary>
    public interface IRoomTypeAmenityService
    {
        /// <summary>
        /// Get all room type amenities
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<RoomTypeAmenity>>> GetAmenitiesByRoomTypeIdAsync(int roomTypeId);

        /// <summary>
        /// Get all amenities for room type
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<BaseAmenity>>> GetAmenityDetailsForRoomTypeAsync(int roomTypeId);

        /// <summary>
        /// Get all default amenities for room type
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<BaseAmenity>>> GetDefaultAmenityDetailsForRoomTypeAsync(int roomTypeId);

        /// <summary>
        /// Get room type amenity by room type and amenity id
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response<RoomTypeAmenity>> GetRoomTypeAmenityAsync(int roomTypeId, int amenityId);

        /// <summary>
        /// Add room type amenity
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task<Response<int>> AddAmenityToRoomTypeAsync(int roomTypeId, int amenityId, bool isDefault);

        /// <summary>
        /// Update room type amenity
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task<Response> UpdateRoomTypeAmenityAsync(int roomTypeId, int amenityId, bool isDefault);

        /// <summary>
        /// Remove room type amenity
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response> RemoveAmenityFromRoomTypeAsync(int roomTypeId, int amenityId);

        /// <summary>
        /// Set all room type amenities default status
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task<Response> SetAllRoomTypeAmenitiesDefaultStatusAsync(int roomTypeId, bool isDefault);

        // Helper methods for room type template management

        /// <summary>
        /// Copy amenities from one room type to another
        /// </summary>
        /// <param name="sourceRoomTypeId"></param>
        /// <param name="targetRoomTypeId"></param>
        /// <param name="replaceExisting"></param>
        /// <returns></returns>
        Task<Response> CopyAmenitiesFromRoomTypeAsync(int sourceRoomTypeId, int targetRoomTypeId, bool replaceExisting);

        /// <summary>
        /// Get room types with amenity
        /// </summary>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<RoomType>>> GetRoomTypesWithAmenityAsync(int amenityId);
    }
}
