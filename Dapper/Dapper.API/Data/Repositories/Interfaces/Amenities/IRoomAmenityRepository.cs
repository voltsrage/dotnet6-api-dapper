using Dapper.API.Entities.Amentities;

namespace Dapper.API.Data.Repositories.Interfaces.Amenities
{
    /// <summary>
    /// Interface for Room Amenity Repository
    /// </summary>
    public interface IRoomAmenityRepository
    {
        /// <summary>
        /// Get all room amenities by room id
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoomAmenity>> GetByRoomIdAsync(int roomId);

        /// <summary>
        /// Get all amenities for room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<IEnumerable<BaseAmenity>> GetAmenitiesForRoomAsync(int roomId);

        /// <summary>
        /// Get all overridden amenities for room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<IEnumerable<BaseAmenity>> GetOverriddenAmenitiesForRoomAsync(int roomId);

        /// <summary>
        /// Get room amenity by room and amenity id
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<RoomAmenity> GetByRoomAndAmenityIdAsync(int roomId, int amenityId);

        /// <summary>
        /// Add room amenity
        /// </summary>
        /// <param name="roomAmenity"></param>
        /// <returns></returns>
        Task<int> AddRoomAmenityAsync(RoomAmenity roomAmenity);

        /// <summary>
        /// Update room amenity
        /// </summary>
        /// <param name="roomAmenity"></param>
        /// <returns></returns>
        Task UpdateRoomAmenityAsync(RoomAmenity roomAmenity);

        /// <summary>
        /// Remove room amenity
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task RemoveRoomAmenityAsync(int roomId, int amenityId);

        /// <summary>
        /// Reset room to room type defaults 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task ResetRoomToRoomTypeDefaultsAsync(int roomId);

        /// <summary>
        /// Method to get all amenities for a room, combining room-specific and room type defaults
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<IEnumerable<IAmenity>> GetAllEffectiveAmenitiesForRoomAsync(Guid roomId);
    }
}
