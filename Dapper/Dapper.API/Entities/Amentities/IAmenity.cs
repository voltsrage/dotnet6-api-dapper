using Dapper.API.Dtos.Common;

namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// Interface for all amenities
    /// </summary>
    public interface IAmenity
    {
        /// <summary>
        /// Numeric Id of amenity
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Name of amenity`
        /// </summary>
        public string Name { get; }  

        /// <summary>
        /// Description of amenity
        /// </summary>
        public string Description { get;  } 

        /// <summary>
        /// Price modifier for the amenity
        /// </summary>
        public decimal PriceModifier { get;  }

        /// <summary>
        /// Is the amenity standard
        /// </summary>
        public bool IsStandard { get;  }

        /// <summary>
        /// Used for decorator pattern
        /// </summary>
        public string InternalIdentifier { get; }
    }
}
