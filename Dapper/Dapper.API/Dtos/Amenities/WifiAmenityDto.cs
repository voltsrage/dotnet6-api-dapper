namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// Data transfer object for the WIFI amenity
    /// </summary>
    public class WifiAmenityDto
    {
        /// <summary>
        /// Name of the network
        /// </summary>
        public string NetworkName { get; set; }

        /// <summary>
        /// Password for the network
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Speed of the network in Mbps
        /// </summary>
        public int SpeedMbps { get; set; }
    }
}
