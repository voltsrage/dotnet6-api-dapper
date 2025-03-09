using Dapper.API.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Dapper.API.Entities
{
    /// <summary>
    /// Represents a hotel entity.
    /// </summary>
    public class HotelEntity: BaseModel
    {
        /// <summary>
        /// Gets or sets the name of the hotel.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the hotel.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city where the hotel is located.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country where the hotel is located.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the hotel.
        /// </summary>
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the hotel.
        /// </summary>
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}
