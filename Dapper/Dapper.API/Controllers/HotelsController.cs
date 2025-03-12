using Dapper.API.Dtos.Hotels;
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
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(
            ILogger<HotelsController> logger, 
            IHotelService hotelService)
        {
            _logger = logger;
            _hotelService = hotelService;
        }

        /// <summary>
        /// Get all hotels
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortDirection"></param>    
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Response<PaginatedResult<Hotel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHotels(
            int page = 1,
            int pageSize = 10, 
            string? searchTerm = null,
            string? sortColumn = "Id",
            string? sortDirection = "asc",
            CancellationToken cancellationToken = default)
        {
            var result = new Response<PaginatedResult<Hotel>>();
            try
            {
                var pagination = new PaginationRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection
                };

                result = await _hotelService.GetAll(pagination, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get all hotels with their rooms
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Optional search term</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>
        /// <param name="country">Filter by country</param>
        /// <param name="city">Filter by city</param>
        /// <returns>Paginated list of hotels with their rooms</returns>
        [HttpGet("withRooms")]
        [ProducesResponseType(typeof(Response<PaginatedResult<HotelWithRooms>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHotelsWithRooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] string? country = null,
            [FromQuery] string? city = null,
            CancellationToken cancellationToken = default)
        {
            var result = new Response<PaginatedResult<HotelWithRooms>>();
            try
            {
                var request = new PaginationRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    Filters = new Dictionary<string, string>()
                };

                // Add filters conditionally
                if (!string.IsNullOrEmpty(country))
                    request.Filters.Add("country", country);

                if (!string.IsNullOrEmpty(city))
                    request.Filters.Add("city", city);

                result = await _hotelService.GetHotelsWithRoomsAsync(request, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotels with rooms retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get a hotel with its rooms by ID
        /// </summary>
        /// <param name="hotelId">The hotel ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The hotel with its rooms</returns>
        [HttpGet("{hotelId}/withRooms")]
        [ProducesResponseType(typeof(Response<HotelWithRooms>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelWithRoomsById(int hotelId, CancellationToken cancellationToken = default)
        {
            var result = new Response<HotelWithRooms>();
            try
            {
                result = await _hotelService.GetHotelWithRoomsByIdAsync(hotelId, cancellationToken);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel with rooms retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get hotel by id
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [HttpGet("{hotelId}")]
        [ProducesResponseType(typeof(Response<Hotel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelById(int hotelId)
        {
            var result = new Response<Hotel>();
            try
            {
                result = await _hotelService.GetHotelById(hotelId);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get hotels by ids
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        [HttpGet("byIds/{hotelIds}")]
        [ProducesResponseType(typeof(Response<IEnumerable<Hotel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelsByIds(string hotelIds)
        {
            var result = new Response<IEnumerable<Hotel>>();
            try
            {
                if (string.IsNullOrEmpty(hotelIds))
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "No IDs provided";
                    return StatusCode((int)HttpStatusCode.BadRequest, result);
                }

                var idList = hotelIds.Split(',').Select(int.Parse).ToList();

                result = await _hotelService.GetByIdsAsync(idList, CancellationToken.None);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Get hotel by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(Response<Hotel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelByName(string name)
        {
            var result = new Response<Hotel>();
            try
            {
                result = await _hotelService.GetHotelByName(name);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel retrieval");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Create a new hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<Hotel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddHotel([FromBody] AddEditHotel model)
        {
            var result = new Response<Hotel>();
            try
            {
                result = await _hotelService.AddHotel(model);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel creation");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Create a new hotel with rooms
        /// </summary>
        /// <param name="model">Hotel with rooms data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created hotel with rooms</returns>
        [HttpPost("withRooms")]
        [ProducesResponseType(typeof(Response<HotelWithRooms>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHotelWithRooms([FromBody] AddHotelWithRooms model, CancellationToken cancellationToken = default)
        {
            var result = new Response<HotelWithRooms>();
            try
            {
                result = await _hotelService.CreateHotelWithRoomsAsync(model, cancellationToken);

                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }

                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel with rooms creation");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Creates multiple hotels in a single transaction
        /// </summary>
        /// <param name="hotels">Collection of hotels to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of created hotels with IDs assigned</returns>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(Response<IEnumerable<Hotel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateManyAsync([FromBody] IEnumerable<AddEditHotel> hotels)
        {
            var result = new Response<IEnumerable<Hotel>>();
            try
            {
                result = await _hotelService.CreateManyAsync(hotels, CancellationToken.None);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel creation");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }



        /// <summary>
        /// Update hotel by id
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{hotelId}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateHotel(int hotelId, [FromBody] AddEditHotel model)
        {
            var result = new Response<bool>();
            try
            {
                result = await _hotelService.UpdateHotel(hotelId, model);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel update");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Delete hotel by id
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [HttpDelete("{hotelId}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotelById(int hotelId)
        {
            var result = new Response<bool>();
            try
            {
                result = await _hotelService.DeleteHotel(hotelId);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel deletion");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Deletes multiple hotels in a single transaction
        /// </summary>
        /// <param name="ids">Collection of hotel IDs to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the bulk delete operation</returns>
        [HttpDelete("batch")]
        [ProducesResponseType(typeof(Response<BulkDeleteResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteManyAsync([FromBody] IEnumerable<int> ids)
        {
            var result = new Response<BulkDeleteResult>();
            try
            {
                result = await _hotelService.DeleteManyAsync(ids, CancellationToken.None);
                if (result.StatusCode != null)
                {
                    return StatusCode(result.StatusCode.Value, result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during hotel deletion");
                result.ErrorMessage = ex.Message;
                throw;
            }
        }
    }
}
