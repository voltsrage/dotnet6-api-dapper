namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// DTO for RoomAmenity entity
    /// </summary>
    public class RoomAmenityDto
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

        /// <summary>
        /// For displaying room details
        /// </summary>
        public AmenityDto Amenity { get; set; } // For displaying amenity details
    }
}
