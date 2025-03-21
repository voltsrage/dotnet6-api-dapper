
using Dapper.API.Entities.Common;

namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// Base class for all amenities
    /// </summary>
    public abstract class BaseAmenity : BaseEntity, IAmenity
    {
        /// <summary>
        /// Name of amenity`
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description of amenity
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Price modifier for the amenity
        /// </summary>
        public decimal PriceModifier { get; private set; }

        /// <summary>
        /// Is the amenity standard
        /// </summary>
        public bool IsStandard { get; private set; }

        /// <summary>
        /// Used for decorator pattern
        /// </summary>
        public string InternalIdentifier { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Constructor for the BaseAmenity class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseAmenity(string name, string description, decimal priceModifier, bool isStandard)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            PriceModifier = priceModifier;
            IsStandard = isStandard;
        }

        /// <summary>
        /// Updates the details of the amenity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UpdateDetails(string name, string description, decimal priceModifier)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            PriceModifier = priceModifier;
        }

        /// <summary>
        /// Sets the amenity as standard
        /// </summary>
        /// <param name="isStandard"></param>
        public void SetAsStandard(bool isStandard)
        {
            IsStandard = isStandard;
        }

    }
}
