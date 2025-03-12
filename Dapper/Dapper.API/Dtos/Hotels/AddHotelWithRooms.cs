using Dapper.API.Dtos.Rooms;

namespace Dapper.API.Dtos.Hotels
{
    /// <summary>
    /// DTO for creating a hotel with its rooms
    /// </summary>
    public class AddHotelWithRooms
    {
        /// <summary>
        /// Hotel data to create
        /// </summary>
        public AddEditHotel Hotel { get; set; }

        /// <summary>
        /// List of rooms to create for the hotel
        /// </summary>
        public IEnumerable<AddEditRoom> Rooms { get; set; }
    }
}
