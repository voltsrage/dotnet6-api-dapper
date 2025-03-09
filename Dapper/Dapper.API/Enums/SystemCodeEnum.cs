using Dapper.API.Enums.EnumBase;
using System.Net;

namespace Dapper.API.Enums
{
    /// <summary>
    /// Allows developer to create error codes for system logic in a central place
    /// </summary>
    public class SystemCodeEnum : Enumeration<SystemCodeEnum>
    {

        #region Hotel Related Codes (3000-3099)

        // Success Codes
        /// <summary>
        /// Hotel created successfully
        /// </summary>
        public static readonly SystemCodeEnum HotelCreated = new(3000, "Hotel Created Successfully", HttpStatusCode.Created);

        /// <summary>
        /// Hotel updated successfully
        /// </summary>
        public static readonly SystemCodeEnum HotelUpdated = new(3001, "Hotel Updated Successfully", HttpStatusCode.OK);

        /// <summary>
        /// Hotel deleted successfully
        /// </summary>
        public static readonly SystemCodeEnum HotelDeleted = new(3002, "Hotel Deleted Successfully", HttpStatusCode.NoContent);

        // Error Codes
        /// <summary>
        /// Hotel not found
        /// </summary>
        public static readonly SystemCodeEnum HotelNotFound = new(3050, "Hotel Not Found", HttpStatusCode.NotFound);

        /// <summary>
        /// Hotel already exists
        /// </summary>
        public static readonly SystemCodeEnum HotelAlreadyExists = new(3051, "Hotel Already Exists", HttpStatusCode.Conflict);

        /// <summary>
        /// Invalid hotel data
        /// </summary>
        public static readonly SystemCodeEnum InvalidHotelData = new(3052, "Invalid Hotel Data", HttpStatusCode.BadRequest);

        /// <summary>
        /// Unable to delete hotel with active rooms
        /// </summary>
        public static readonly SystemCodeEnum HotelHasActiveRooms = new(3053, "Cannot Delete Hotel With Active Rooms", HttpStatusCode.Conflict);

        /// <summary>
        /// Invalid hotel contact information
        /// </summary>
        public static readonly SystemCodeEnum InvalidHotelContactInfo = new(3054, "Invalid Hotel Contact Information", HttpStatusCode.BadRequest);

        /// <summary>
        /// Hotel capacity exceeded
        /// </summary>
        public static readonly SystemCodeEnum HotelCapacityExceeded = new(3055, "Hotel Capacity Exceeded", HttpStatusCode.BadRequest);

        #endregion

        private SystemCodeEnum(int value, string name) : base(value, name) { }

        public HttpStatusCode StatusCode { get; private set; }

        private SystemCodeEnum(int value, string name, HttpStatusCode statusCode) : base(value, name)
        {
            StatusCode = statusCode;
        }
    }
}
