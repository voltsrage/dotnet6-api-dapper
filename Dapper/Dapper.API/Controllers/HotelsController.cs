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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Response<PaginatedResult<Hotel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHotels(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = new Response<PaginatedResult<Hotel>>();
            try
            {
                result = await _hotelService.GetAll(page,pageSize,cancellationToken);
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
    }
}
