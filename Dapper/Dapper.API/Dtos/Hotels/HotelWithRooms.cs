using Dapper.API.Dtos.Rooms;

namespace Dapper.API.Dtos.Hotels
{
    /// <summary>
    /// Represents an instance of a hotel and the rooms inside it
    /// </summary>
    public class HotelWithRooms
    {
        /// <summary>
        /// Gets or sets the unique identifier for the hotel.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the hotel.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the hotel.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city where the hotel is located.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country where the hotel is located.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the hotel.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the hotel.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the create time for the hotel
        /// </summary>
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// Gets or sets the rooms inside of this hotel
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
