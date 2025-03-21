namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// Decorator class for amenities
    /// </summary>
    public abstract class AmenityDecorator : IAmenity
    {
        /// <summary>
        /// Base amenity
        /// </summary>
        protected readonly IAmenity _baseAmenity;

        /// <summary>
        /// Id of the amenity
        /// </summary>
        public int Id => _baseAmenity.Id;
        public virtual string Name => _baseAmenity.Name;
        public virtual string Description => _baseAmenity.Description;
        public virtual decimal PriceModifier => _baseAmenity.PriceModifier;
        public virtual bool IsStandard => _baseAmenity.IsStandard;
        public string InternalIdentifier { get; } = Guid.NewGuid().ToString();

        protected AmenityDecorator(IAmenity baseAmenity)
        {
            _baseAmenity = baseAmenity ?? throw new ArgumentNullException(nameof(baseAmenity));
        }
    }
}
