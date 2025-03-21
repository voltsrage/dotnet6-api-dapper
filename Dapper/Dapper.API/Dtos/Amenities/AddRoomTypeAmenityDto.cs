namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// Data transfer object for adding a room type amenity
    /// </summary>
    public class AddRoomTypeAmenityDto
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
    }
}
