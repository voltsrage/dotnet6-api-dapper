namespace Dapper.API.Dtos.Amenities
{
    public class RoomServiceAmenityDto
    {
        /// <summary>
        /// Hours available for room service
        /// </summary>
        public int HoursAvailable { get; set; }

        /// <summary>
        /// Is the room service available 24 hours
        /// </summary>
        public bool Is24Hours { get; set; }
    }
}
