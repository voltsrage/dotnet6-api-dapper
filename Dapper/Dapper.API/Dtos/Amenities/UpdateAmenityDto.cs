namespace Dapper.API.Dtos.Amenities
{
    /// <summary>
    /// Data transfer object for updating an amenity
    /// </summary>
    public class UpdateAmenityDto
    {
        /// <summary>
        /// Name of the amenity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the amenity
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Price modifier for the amenity
        /// </summary>
        public decimal PriceModifier { get; set; }

        /// <summary>
        /// Whether the amenity is a standard amenity
        /// </summary>
        public bool IsStandard { get; set; }

        /// <summary>
        /// Special properties for the amenity
        /// </summary>
        public Dictionary<string, object> SpecialProperties { get; set; } // For type-specific properties
    }
}
