using Dapper.API.Dtos.Common;

namespace Dapper.API.Dtos.Rooms
{
    /// <summary>
    /// Represents a room type for dropdown lists and selection.
    /// </summary>
    public class AddEditRoomType
    {

        /// <summary>
        /// Gets or sets the name of the room type.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the room type.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
