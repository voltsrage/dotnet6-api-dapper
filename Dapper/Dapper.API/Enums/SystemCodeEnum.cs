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

        /// <summary>
        /// Hotel creation failed
        /// </summary>
        public static readonly SystemCodeEnum HotelCreationFailed = new(3056, "Hotel Creation Failed", HttpStatusCode.BadRequest);

        /// <summary>
        /// No hotels to create in bulk
        /// </summary>
        public static readonly SystemCodeEnum NoHotelsToCreate = new(3057, "No Hotels To Create", HttpStatusCode.BadRequest);

        /// <summary>
        /// Validation failed for one or more hotels
        /// </summary>
        public static readonly SystemCodeEnum HotelBulkCreateValidationError = new(3058, "Validation failed for one or more hotels", HttpStatusCode.BadRequest);


        #endregion

        #region Room Related Codes (3100-3199)

        // Success Codes
        /// <summary>
        /// Room created successfully
        /// </summary>
        public static readonly SystemCodeEnum RoomCreated = new(3100, "Room Created Successfully", HttpStatusCode.Created);

        /// <summary>
        /// Room updated successfully
        /// </summary>
        public static readonly SystemCodeEnum RoomUpdated = new(3101, "Room Updated Successfully", HttpStatusCode.OK);

        /// <summary>
        /// Room deleted successfully
        /// </summary>
        public static readonly SystemCodeEnum RoomDeleted = new(3102, "Room Deleted Successfully", HttpStatusCode.NoContent);

        /// <summary>
        /// Room status changed successfully
        /// </summary>
        public static readonly SystemCodeEnum RoomStatusChanged = new(3103, "Room Status Changed Successfully", HttpStatusCode.OK);

        // Error Codes
        /// <summary>
        /// Room not found
        /// </summary>
        public static readonly SystemCodeEnum RoomNotFound = new(3150, "Room Not Found", HttpStatusCode.NotFound);

        /// <summary>
        /// Room already exists
        /// </summary>
        public static readonly SystemCodeEnum RoomAlreadyExists = new(3151, "Room Already Exists", HttpStatusCode.Conflict);

        /// <summary>
        /// Invalid room data
        /// </summary>
        public static readonly SystemCodeEnum InvalidRoomData = new(3152, "Invalid Room Data", HttpStatusCode.BadRequest);

        /// <summary>
        /// Room is currently occupied
        /// </summary>
        public static readonly SystemCodeEnum RoomOccupied = new(3153, "Room Is Currently Occupied", HttpStatusCode.Conflict);

        /// <summary>
        /// Room is under maintenance
        /// </summary>
        public static readonly SystemCodeEnum RoomUnderMaintenance = new(3154, "Room Is Under Maintenance", HttpStatusCode.Conflict);

        /// <summary>
        /// Room capacity exceeded
        /// </summary>
        public static readonly SystemCodeEnum RoomCapacityExceeded = new(3155, "Room Capacity Exceeded", HttpStatusCode.BadRequest);

        /// <summary>
        /// Room is not available for booking
        /// </summary>
        public static readonly SystemCodeEnum RoomNotAvailable = new(3156, "Room Not Available For Booking", HttpStatusCode.Conflict);

        /// <summary>
        /// Invalid room price
        /// </summary>
        public static readonly SystemCodeEnum InvalidRoomPrice = new(3157, "Invalid Room Price", HttpStatusCode.BadRequest);

        /// <summary>
        /// Room belongs to a different hotel
        /// </summary>
        public static readonly SystemCodeEnum RoomBelongsToDifferentHotel = new(3158, "Room Belongs To A Different Hotel", HttpStatusCode.Conflict);


        /// <summary>
        /// Room creation failed
        /// </summary>
        public static readonly SystemCodeEnum RoomCreationFailed = new(3159, "Room Creation Failed", HttpStatusCode.BadRequest);

        #endregion

        private SystemCodeEnum(int value, string name) : base(value, name) { }

        public HttpStatusCode StatusCode { get; private set; }

        private SystemCodeEnum(int value, string name, HttpStatusCode statusCode) : base(value, name)
        {
            StatusCode = statusCode;
        }
    }
}
