using Dapper.API.Entities.Amentities;

namespace Dapper.API.Data.Repositories.Interfaces.Amenities
{
    /// <summary>
    /// Interface for Amenity Repository
    /// </summary>
    public interface IAmenityRepository
    {
        /// <summary>
        /// Get all amenities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<BaseAmenity>> GetAllAmenitiesAsync();

        /// <summary>
        /// Get amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseAmenity> GetAmenityByIdAsync(int id);

        /// <summary>
        /// Get amenity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<BaseAmenity> GetAmenityByNameAsync(string name);

        /// <summary>
        /// Get all standard amenities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<BaseAmenity>> GetStandardAmenitiesAsync();

        /// <summary>
        /// Get all decorated amenities by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IAmenity> GetDecoratedAmenityAsync(int id); // Returns amenity with decorators applied

        /// <summary>
        /// Add amenity
        /// </summary>
        /// <param name="amenity"></param>
        /// <returns></returns>
        Task<int> AddAmenityAsync(BaseAmenity amenity);

        /// <summary>
        /// Update amenity
        /// </summary>
        /// <param name="amenity"></param>
        /// <returns></returns>
        Task UpdateAmenityAsync(BaseAmenity amenity);

        /// <summary>
        /// Delete amenity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAmenityAsync(int id);

        // Type-specific amenity methods
        /// <summary>
        /// Get all wifi amenities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<WIFIAmenity>> GetAllWifiAmenitiesAsync();

        /// <summary>
        /// Get wifi amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<WIFIAmenity> GetWifiAmenityByIdAsync(int id);

        /// <summary>
        /// Get all mini bar amenities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MiniBarAmenity>> GetAllMiniBarAmenitiesAsync();

        /// <summary>
        /// Get mini bar amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MiniBarAmenity> GetMiniBarAmenityByIdAsync(int id);

        /// <summary>
        /// Get all room service amenities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RoomServiceAmenity>> GetAllRoomServiceAmenitiesAsync();

        /// <summary>
        /// Get room service amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RoomServiceAmenity> GetRoomServiceAmenityByIdAsync(int id);

        // Decorator-related methods
        /// <summary>
        /// Add premium decorator
        /// </summary>
        /// <param name="decorator"></param>
        /// <returns></returns>
        Task<int> AddPremiumDecoratorAsync(PremiumAmenityDecorator decorator);

        /// <summary>
        /// Add seasonal decorator
        /// </summary>
        /// <param name="decorator"></param>
        /// <returns></returns>
        Task<int> AddSeasonalDecoratorAsync(SeasonalAmenityDecorator decorator);

        /// <summary>
        /// Get all premium decorators for amenity
        /// </summary>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<IEnumerable<PremiumAmenityDecorator>> GetPremiumDecoratorsForAmenityAsync(int amenityId);

        /// <summary>
        /// Get all seasonal decorators for amenity
        /// </summary>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<IEnumerable<SeasonalAmenityDecorator>> GetSeasonalDecoratorsForAmenityAsync(int amenityId);

        /// <summary>
        /// Remove decorator
        /// </summary>
        /// <param name="decoratorId"></param>
        /// <returns></returns>
        Task RemoveDecoratorAsync(int decoratorId);
    }
}
