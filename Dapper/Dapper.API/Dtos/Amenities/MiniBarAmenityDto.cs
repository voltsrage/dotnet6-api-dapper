namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// Data transfer object for the mini bar amenity
    /// </summary>
    public class MiniBarAmenityDto
    {
        /// <summary>
        /// Is the mini bar complimentary
        /// </summary>
        public bool IsComplimentary { get; set; }

        /// <summary>
        /// List of items in the mini bar
        /// </summary>
        public List<string> Items { get; set; } = new List<string>();
    }
}
