namespace Dapper.API.Dtos.Amenities
{
    public class RoomTypeAmenityDto
    {
        /// <summary>
        /// Id of the room type amenity
        /// </summary>
        public int RoomTypeId { get; set; }

        /// <summary>
        /// Id of the amenity
        /// </summary>
        public int AmenityId { get; set; }

        /// <summary>
        /// Is the amenity included in the room type
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Amenity details
        /// </summary>
        public AmenityDto Amenity { get; set; } // For displaying amenity details
    }
}
