namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// Data transfer object for the seasonal amenity
    /// </summary>
    public class SeasonalAmenityDto
    {
        /// <summary>
        /// Id of the base amenity
        /// </summary>
        public int BaseAmenityId { get; set; }

        /// <summary>
        /// Seasonal amenity name
        /// </summary>
        public string Season { get; set; }

        /// <summary>
        /// Start date of the seasonal amenity
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the seasonal amenity
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Price adjustment for the seasonal amenity
        /// </summary>
        public decimal SeasonalPriceAdjustment { get; set; }
    }
}
