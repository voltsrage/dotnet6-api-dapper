using Dapper.API.Dtos.Rooms;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;
using Dapper.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Dapper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(
            ILogger<RoomsController> logger,
            IRoomService roomService)
        {
            _logger = logger;
            _roomService = roomService;
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Optional search term</param>
        /// <param name="country">Optional country filter</param>
        /// <param name="city">Optional city filter</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>   
        /// <param name="guests">Number of guests</param>
        /// <param name="hotelId">Optional hotel ID filter</param>
        /// <param name="roomTypeId">Optional room type ID filter</param> 
        /// <param name="minPrice">Minimum price per night</param>
        /// <param name="maxPrice">Maximum price per night</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of rooms</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Response<PaginatedResult<Room>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRooms(
            int page = 1,
            int pageSize = 10,
            int guests = 1,
            int? hotelId = null,
            int? roomTypeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? searchTerm = null,
            string? country = null,
            string? city = null,
            string? sortColumn = "Id",
            string? sortDirection = "asc",
            CancellationToken cancellationToken = default)
        {
            var result = new Response<PaginatedResult<Room>>();
            try
            {
                var pagination = new PaginationRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    Filters = new Dictionary<string, string>
                    {
                        // Always filter for available rooms with sufficient capacity
                        { "isAvailable", "true" },
                        { "maxOccupancy__gte"
                        , guests.ToString() }
                    }
                };

                // Add optional filters
                if (hotelId.HasValue)
                    pagination.Filters.Add("hotelId", hotelId.Value.ToString());

                if (roomTypeId.HasValue)
                    pagination.Filters.Add("roomTypeId", roomTypeId.Value.ToString());

                if (minPrice.HasValue)
                    pagination.Filters.Add("priceMin__gte", minPrice.Value.ToString());

                if (maxPrice.HasValue)
                    pagination.Filters.Add("priceMax__lte", maxPrice.Value.ToString());

                if (!string.IsNullOrEmpty(country))
                    pagination.Filters.Add("country", country);

                if (!string.IsNullOrEmpty(city))
                    pagination.Filters.Add("city", city);

                result = await _roomService.GetAllAsync(pagination, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during rooms retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get room by id
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Room if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<Room>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomById(int id, CancellationToken cancellationToken = default)
        {
            var result = new Response<Room>();
            try
            {
                result = await _roomService.GetByIdAsync(id, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during room retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get rooms by hotel id
        /// </summary>
        /// <param name="hotelId">The hotel ID</param>
        /// <param name="roomTypeId">Optional room type ID filter</param> 
        /// <param name="minPrice">Minimum price per night</param>
        /// <param name="maxPrice">Maximum price per night</param>
        /// <param name="page">Page number</param>  
        /// <param name="guests">Number of guests</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Optional search term</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>    
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of rooms for a hotel</returns>
        [HttpGet("hotel/{hotelId}")]
        [ProducesResponseType(typeof(Response<PaginatedResult<Room>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomsByHotelId(
            int hotelId,
            int page = 1,
            int pageSize = 10,
            int guests = 1,
            int? roomTypeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? searchTerm = null,
            string? sortColumn = "Id",
            string? sortDirection = "asc",
            CancellationToken cancellationToken = default)
        {
            var result = new Response<PaginatedResult<Room>>();
            try
            {
                var pagination = new PaginationRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    Filters = new Dictionary<string, string>
                    {
                        // Always filter for available rooms with sufficient capacity
                        { "isAvailable", "true" },
                        { "maxOccupancy__gte"
                        , guests.ToString() }
                    }
                };

                // Add optional filters

                if (roomTypeId.HasValue)
                    pagination.Filters.Add("roomTypeId", roomTypeId.Value.ToString());

                if (minPrice.HasValue)
                    pagination.Filters.Add("priceMin__gte", minPrice.Value.ToString());

                if (maxPrice.HasValue)
                    pagination.Filters.Add("priceMax__lte", maxPrice.Value.ToString());

                result = await _roomService.GetByHotelIdAsync(hotelId, pagination, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during rooms retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Create a new room
        /// </summary>
        /// <param name="model">Room data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created room with ID assigned</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<Room>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoom([FromBody] AddEditRoom model, CancellationToken cancellationToken = default)
        {
            var result = new Response<Room>();
            try
            {
                result = await _roomService.CreateAsync(model, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during room creation");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Update an existing room
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="model">Updated room data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated room</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Response<Room>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] AddEditRoom model, CancellationToken cancellationToken = default)
        {
            var result = new Response<Room>();
            try
            {
                result = await _roomService.UpdateAsync(id, model, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during room update");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Delete a room
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if deleted; otherwise false</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom(int id, CancellationToken cancellationToken = default)
        {
            var result = new Response<bool>();
            try
            {
                result = await _roomService.DeleteAsync(id, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during room deletion");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Update room availability status
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <param name="isAvailable">New availability status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if updated; otherwise false</returns>
        [HttpPatch("{id}/availability")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoomAvailability(int id, [FromBody] bool isAvailable, CancellationToken cancellationToken = default)
        {
            var result = new Response<bool>();
            try
            {
                result = await _roomService.UpdateAvailabilityAsync(id, isAvailable, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during room availability update");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get all room types
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of room types</returns>
        [HttpGet("types")]
        [ProducesResponseType(typeof(Response<IEnumerable<RoomType>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoomTypes(CancellationToken cancellationToken = default)
        {
            var result = new Response<IEnumerable<RoomType>>();
            try
            {
                result = await _roomService.GetRoomTypesAsync(cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during room types retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }
    }
}