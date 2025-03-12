﻿using Dapper.API.Dtos.Common;

namespace Dapper.API.Dtos.Rooms
{
    /// <summary>
    /// Represents a room data transfer object.
    /// </summary>
    public class Room : BaseModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the hotel to which the room belongs.
        /// </summary>
        public int HotelId { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        public string RoomNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the room (e.g., Standard, Deluxe, Suite).
        /// </summary>
        public int RoomTypeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the room type.
        /// </summary>
        public string RoomTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price per night for the room.
        /// </summary>
        public decimal PricePerNight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the room is available.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of guests that can occupy the room.
        /// </summary>
        public int MaxOccupancy { get; set; }

        /// <summary>
        /// Gets or sets the name of the hotel to which the room belongs.
        /// </summary>
        public string HotelName { get; set; } = string.Empty;
    }
}
