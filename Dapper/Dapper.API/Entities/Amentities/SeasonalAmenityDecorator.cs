namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// Decorator class for seasonal amenities
    /// </summary>
    public class SeasonalAmenityDecorator : AmenityDecorator
    {
        /// <summary>
        /// Season for the amenity
        /// </summary>
        public string Season { get; private set; }

        /// <summary>
        /// Start date for the season
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// End date for the season
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Price adjustment for the season
        /// </summary>
        public decimal SeasonalPriceAdjustment { get; private set; }

        /// <summary>
        /// Constructor for the SeasonalAmenityDecorator class
        /// </summary>
        /// <param name="baseAmenity"></param>
        /// <param name="season"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="seasonalPriceAdjustment"></param>
        public SeasonalAmenityDecorator(IAmenity baseAmenity, string season, DateTime startDate, DateTime endDate, decimal seasonalPriceAdjustment)
            : base(baseAmenity)
        {
            Season = season;
            StartDate = startDate;
            EndDate = endDate;
            SeasonalPriceAdjustment = seasonalPriceAdjustment;
        }

        /// <summary>
        /// Price modifier for the seasonal amenity
        /// </summary>
        public override decimal PriceModifier
        {
            get
            {
                var now = DateTime.UtcNow;
                if (now >= StartDate && now <= EndDate)
                {
                    return _baseAmenity.PriceModifier + SeasonalPriceAdjustment;
                }
                return _baseAmenity.PriceModifier;
            }
        }

        /// <summary>
        /// Description of the seasonal amenity
        /// </summary>
        public override string Description => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate
            ? $"{_baseAmenity.Description} (Special: {Season})"
            : _baseAmenity.Description;
    }
}
