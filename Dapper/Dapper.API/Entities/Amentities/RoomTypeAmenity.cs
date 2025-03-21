using Dapper.API.Entities.Common;

namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// Represents the relationship between a room type and an amenity.
    /// </summary>
    public class RoomTypeAmenity : BaseEntity
    {

        /// <summary>
        /// The unique identifier of the room type.
        /// </summary>
        public int RoomTypeId { get; private set; }

        /// <summary>
        /// The unique identifier of the amenity.
        /// </summary>
        public int AmenityId { get; private set; }

        /// <summary>
        /// Indicates whether the amenity is the default for the room type.
        /// </summary>
        public bool IsDefault { get; private set; }

        /// <summary>
        /// Store internal identifiers for the decorators
        /// </summary>
        public string AmenityInternalId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomTypeAmenity"/> class.
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="amenityId"></param>
        /// <param name="amenityInternalId"></param>
        /// <param name="isDefault"></param>
        public RoomTypeAmenity(int roomTypeId, int amenityId, string amenityInternalId, bool isDefault)
        {
            RoomTypeId = roomTypeId;
            AmenityId = amenityId;
            IsDefault = isDefault;
            AmenityInternalId = amenityInternalId;
        }

        /// <summary>
        /// Sets the amenity as the default for the room type.
        /// </summary>
        /// <param name="isDefault"></param>
        public void SetAsDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }
}
