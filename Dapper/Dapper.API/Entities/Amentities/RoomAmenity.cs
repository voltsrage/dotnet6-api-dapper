using Dapper.API.Entities.Common;

namespace Dapper.API.Entities.Amentities
{

    /// <summary>
    /// Represents a room amenity
    /// </summary>
    public class RoomAmenity : BaseEntity
    {
        /// <summary>
        /// Gets the unique identifier of the room.
        /// </summary>
        public int RoomId { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the amenity.
        /// </summary>
        public int AmenityId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the amenity is overridden.
        /// </summary>
        public bool IsOverridden { get; private set; } // Indicates if this overrides the room type's default

        /// <summary>
        /// Gets the custom price modifier for the amenity.
        /// </summary>
        public decimal CustomPriceModifier { get; private set; } // Optional: For room-specific pricing

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAmenity"/> class.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="amenityId"></param>
        /// <param name="isOverridden"></param>
        /// <param name="customPriceModifier"></param>
        public RoomAmenity(int roomId, int amenityId, bool isOverridden, decimal customPriceModifier = 0)
        {
            RoomId = roomId;
            AmenityId = amenityId;
            IsOverridden = isOverridden;
            CustomPriceModifier = customPriceModifier;
        }

        /// <summary>
        /// Sets the amenity as overridden.
        /// </summary>
        /// <param name="isOverridden"></param>
        public void SetOverride(bool isOverridden)
        {
            IsOverridden = isOverridden;
        }

        /// <summary>
        /// Updates the custom price modifier for the amenity.
        /// </summary>
        /// <param name="customPriceModifier"></param>
        public void UpdateCustomPrice(decimal customPriceModifier)
        {
            CustomPriceModifier = customPriceModifier;
        }
    }
}
