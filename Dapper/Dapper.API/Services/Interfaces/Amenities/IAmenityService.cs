using Dapper.API.Entities.Amentities;
using Dapper.API.Models;

namespace Dapper.API.Services.Interfaces.Amenities
{
    /// <summary>
    /// Interface for Amenity Service
    /// </summary>
    public interface IAmenityService
    {
        /// <summary>
        /// Get amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<BaseAmenity>> GetAmenityByIdAsync(int id);

        /// <summary>
        /// Get all amenities
        /// </summary>
        /// <returns></returns>
        Task<Response<IEnumerable<BaseAmenity>>> GetAllAmenitiesAsync();

        /// <summary>
        /// Get amenity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Response<BaseAmenity>> GetAmenityByNameAsync(string name);

        /// <summary>
        /// Get all standard amenities
        /// </summary>
        /// <returns></returns>
        Task<Response<IEnumerable<BaseAmenity>>> GetStandardAmenitiesAsync();

        /// <summary>
        /// Get all decorated amenities by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<IAmenity>> GetDecoratedAmenityAsync(int id);

        /// <summary>
        /// Add amenity
        /// </summary>
        /// <param name="amenityType"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task<Response<int>> CreateAmenityAsync(string amenityType, Dictionary<string, object> properties);

        /// <summary>
        /// Update amenity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task<Response> UpdateAmenityAsync(int id, Dictionary<string, object> properties);

        /// <summary>
        /// Delete amenity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response> DeleteAmenityAsync(int id);

        // Type-specific amenity methods

        /// <summary>
        /// Get all wifi amenities
        /// </summary>
        /// <returns></returns>
        Task<Response<IEnumerable<WIFIAmenity>>> GetAllWifiAmenitiesAsync();

        /// <summary>
        /// Get wifi amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<WIFIAmenity>> GetWifiAmenityByIdAsync(int id);

        /// <summary>
        /// Create wifi amenity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="networkName"></param>
        /// <param name="password"></param>
        /// <param name="speedMbps"></param>
        /// <returns></returns>
        Task<Response<int>> CreateWifiAmenityAsync(string name, string description, decimal priceModifier, bool isStandard,
                                         string networkName, string password, int speedMbps);

        /// <summary>
        /// Update wifi amenity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="networkName"></param>
        /// <param name="password"></param>
        /// <param name="speedMbps"></param>
        /// <returns></returns>
        Task<Response> UpdateWifiAmenityAsync(int id, string name, string description, decimal priceModifier, bool isStandard,
                                   string networkName, string password, int speedMbps);

        /// <summary>
        /// Get all mini bar amenities
        /// </summary>
        /// <returns></returns>
        Task<Response<IEnumerable<MiniBarAmenity>>> GetAllMiniBarAmenitiesAsync();

        /// <summary>
        /// Get mini bar amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<MiniBarAmenity>> GetMiniBarAmenityByIdAsync(int id);

        /// <summary>
        /// Create mini bar amenity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="isComplimentary"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<Response<int>> CreateMiniBarAmenityAsync(string name, string description, decimal priceModifier, bool isStandard,
                                            bool isComplimentary, List<string> items);

        /// <summary>
        /// Update mini bar amenity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="isComplimentary"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<Response> UpdateMiniBarAmenityAsync(int id, string name, string description, decimal priceModifier, bool isStandard,
                                      bool isComplimentary, List<string> items);

        /// <summary>
        /// Get all room service amenities
        /// </summary>
        /// <returns></returns>
        Task<Response<IEnumerable<RoomServiceAmenity>>> GetAllRoomServiceAmenitiesAsync();

        /// <summary>
        /// Get room service amenity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Response<RoomServiceAmenity>> GetRoomServiceAmenityByIdAsync(int id);

        /// <summary>
        /// Create room service amenity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="hoursAvailable"></param>
        /// <param name="is24Hours"></param>
        /// <returns></returns>
        Task<Response<int>> CreateRoomServiceAmenityAsync(string name, string description, decimal priceModifier, bool isStandard,
                                                int hoursAvailable, bool is24Hours);

        /// <summary>
        /// Update room service amenity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="hoursAvailable"></param>
        /// <param name="is24Hours"></param>
        /// <returns></returns>
        Task<Response> UpdateRoomServiceAmenityAsync(int id, string name, string description, decimal priceModifier, bool isStandard,
                                          int hoursAvailable, bool is24Hours);

        // Decorator-related methods

        /// <summary>
        /// Add premium decorator
        /// </summary>
        /// <param name="baseAmenityId"></param>
        /// <param name="premiumFeature"></param>
        /// <param name="additionalCost"></param>
        /// <returns></returns>
        Task<Response<int>> AddPremiumDecoratorAsync(int baseAmenityId, string premiumFeature, decimal additionalCost);

        /// <summary>
        /// Add seasonal decorator
        /// </summary>
        /// <param name="baseAmenityId"></param>
        /// <param name="season"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="seasonalPriceAdjustment"></param>
        /// <returns></returns>
        Task<Response<int>> AddSeasonalDecoratorAsync(int baseAmenityId, string season, DateTime startDate, DateTime endDate, decimal seasonalPriceAdjustment);

        /// <summary>
        /// Get all premium decorators for amenity
        /// </summary>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<PremiumAmenityDecorator>>> GetPremiumDecoratorsForAmenityAsync(int amenityId);

        /// <summary>
        /// Get all seasonal decorators for amenity
        /// </summary>
        /// <param name="amenityId"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<SeasonalAmenityDecorator>>> GetSeasonalDecoratorsForAmenityAsync(int amenityId);

        /// <summary>
        /// Remove premium decorator
        /// </summary>
        /// <param name="decoratorId"></param>
        /// <returns></returns>
        Task<Response> RemoveDecoratorAsync(int decoratorId);
    }
}
