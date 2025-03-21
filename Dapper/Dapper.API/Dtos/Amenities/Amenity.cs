using Dapper.API.Dtos.Common;

namespace Dapper.API.Dtos.Amenities
{
    public class Amenity : BaseModel
    {
        /// <summary>
        /// Name of amenity`
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of amenity
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
