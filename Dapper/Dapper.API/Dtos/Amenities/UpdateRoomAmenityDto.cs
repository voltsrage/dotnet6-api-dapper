namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// DTO for updating a room amenity
    /// </summary>
    public class UpdateRoomAmenityDto
    {
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
