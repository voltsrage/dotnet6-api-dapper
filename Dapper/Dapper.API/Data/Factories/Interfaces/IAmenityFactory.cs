using Dapper.API.Entities.Amentities;
using Dapper.API.Enums.StandardEnums;

namespace Dapper.API.Data.Factories.Interfaces
{
    /// <summary>
    /// Interface for the Amenity Factory
    /// </summary>
    public interface IAmenityFactory
    {
        /// <summary>
        /// Create an amenity
        /// </summary>
        /// <param name="amenityType"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        BaseAmenity CreateAmenity(AmenityTypeEnum amenityType, Dictionary<string, object> properties);

        /// <summary>
        /// Create a decorator
        /// </summary>
        /// <param name="decoratorType"></param>
        /// <param name="baseAmenity"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        IAmenity CreateDecorator(DecoratorTypeEnum decoratorType, IAmenity baseAmenity, Dictionary<string, object> properties);
    }
}
