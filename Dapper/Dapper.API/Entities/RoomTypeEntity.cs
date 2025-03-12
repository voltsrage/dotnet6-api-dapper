using Dapper.API.Dtos.Common;

namespace Dapper.API.Entities
{
    /// <summary>
    /// Represents a room type in the hotel
    /// </summary>
    public class RoomTypeEntity : BaseModel
    {
        /// <summary>
        /// Name of room type
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of room type
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
