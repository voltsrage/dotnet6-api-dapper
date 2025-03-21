namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// Data transfer object for updating a room type amenity
    /// </summary>
    public class UpdateRoomTypeAmenityDto
    {
        /// <summary>
        /// Is the amenity included in the room type
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
