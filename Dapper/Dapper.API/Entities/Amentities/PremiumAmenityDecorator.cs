namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// Decorator class for premium amenities
    /// </summary>
    public class PremiumAmenityDecorator: AmenityDecorator
    {
        /// <summary>
        /// Name of the premium feature
        /// </summary>
        public string PremiumFeature { get; private set; }

        /// <summary>
        /// Additional cost for the premium feature
        /// </summary>
        public decimal AdditionalCost { get; private set; }

        /// <summary>
        /// Constructor for the PremiumAmenityDecorator class
        /// </summary>
        /// <param name="baseAmenity"></param>
        /// <param name="premiumFeature"></param>
        /// <param name="additionalCost"></param>
        public PremiumAmenityDecorator(IAmenity baseAmenity, string premiumFeature, decimal additionalCost)
            : base(baseAmenity)
        {
            PremiumFeature = premiumFeature;
            AdditionalCost = additionalCost;
        }

        /// <summary>
        /// Price modifier for the premium amenity
        /// </summary>
        public override decimal PriceModifier => _baseAmenity.PriceModifier + AdditionalCost;

        /// <summary>
        /// Description of the premium amenity
        /// </summary>
        public override string Description => $"{_baseAmenity.Description} (Premium: {PremiumFeature})";
    }
}
