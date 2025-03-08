namespace Dapper.API.Dtos.Hotels
{
    /// <summary>
    /// Use for adding or editing a hotel.
    /// </summary>
    public class AddEditHotel
    {
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
    }
}
