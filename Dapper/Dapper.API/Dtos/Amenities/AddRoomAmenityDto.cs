namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// DTO for adding a room amenity
    /// </summary>
    public class AddRoomAmenityDto
    {
        /// <summary>
        /// Room Id
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Amenity Id
        /// </summary>
        public int AmenityId { get; set; }

        /// <summary>
        /// Whether the amenity is overridden
        /// </summary>
        public bool IsOverridden { get; set; }

        /// <summary>
        /// Custom price modifier for the amenity
        /// </summary>
        public decimal CustomPriceModifier { get; set; }
    }
}
