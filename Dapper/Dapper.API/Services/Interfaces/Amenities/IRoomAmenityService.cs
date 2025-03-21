using Dapper.API.Dtos.Rooms;
using Dapper.API.Entities.Amentities;
using Dapper.API.Models;

namespace Dapper.API.Services.Interfaces.Amenities
{
    /// <summary>
    /// Interface for Room Amenity Service
    /// </summary>
    public interface IRoomAmenityService
    {
        /// <summary>
        /// Get all amenities for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<RoomAmenity>>> GetAmenitiesByRoomIdAsync(int roomId);

        /// <summary>
        /// Get all amenities for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<BaseAmenity>>> GetAmenityDetailsForRoomAsync(int roomId);

        /// <summary>
        /// Get all overridden amenities for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<BaseAmenity>>> GetOverriddenAmenityDetailsForRoomAsync(int roomId);

        /// <summary>
        /// Get all amenities for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response<RoomAmenity>> GetRoomAmenityAsync(int roomId, int amenityId);

        /// <summary>
        /// Add an amenity to a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <param name="isOverridden"></param>
        /// <param name="customPriceModifier"></param>
        /// <returns></returns>
        Task<Response<int>> AddAmenityToRoomAsync(int roomId, int amenityId, bool isOverridden, decimal customPriceModifier = 0);

        /// <summary>
        /// Update an amenity for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <param name="isOverridden"></param>
        /// <param name="customPriceModifier"></param>
        /// <returns></returns>
        Task<Response> UpdateRoomAmenityAsync(int roomId, int amenityId, bool isOverridden, decimal customPriceModifier);

        /// <summary>
        /// Remove an amenity from a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response> RemoveAmenityFromRoomAsync(int roomId, int amenityId);

        /// <summary>
        /// Reset all amenities for a room to room type defaults
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<Response> ResetRoomToRoomTypeDefaultsAsync(int roomId);

        /// <summary>
        /// Get all effective amenities for a room (combines room-specific and room type defaults)
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<IAmenity>>> GetAllEffectiveAmenitiesForRoomAsync(int roomId);

        /// <summary>
        /// Calculate total price impact of all amenities for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<Response<decimal>> CalculateTotalAmenityPriceModifierForRoomAsync(int roomId);

        // Helper methods for room amenity management

        /// <summary>
        /// Copy amenities from one room to another
        /// </summary>
        /// <param name="sourceRoomId"></param>
        /// <param name="targetRoomId"></param>
        /// <param name="replaceExisting"></param>
        /// <returns></returns>
        Task<Response> CopyAmenitiesFromRoomAsync(int sourceRoomId, int targetRoomId, bool replaceExisting);

        /// <summary>
        /// Get all rooms with a specific amenity
        /// </summary>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<Room>>> GetRoomsWithAmenityAsync(int amenityId);

        /// <summary>
        /// Apply room type defaults to a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="preserveOverrides"></param>
        /// <returns></returns>
        Task<Response> ApplyRoomTypeDefaultsToRoomAsync(int roomId, bool preserveOverrides);
    }
}
