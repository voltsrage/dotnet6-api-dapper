using Dapper.API.Data.Dapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;
using Dapper.API.Exceptions;
using Dapper.API.Models.Pagination;
using System.Text;

namespace Dapper.API.Data.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly IDapperHandler _dataAccess;
        private readonly ILogger<HotelRepository> _logger;

        public HotelRepository(ILogger<HotelRepository> logger, IDapperHandler dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Add a new hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> AddHotel(HotelEntity model)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(AddHotel)}");

                // Add the parameters necessary to create a new model
                DynamicParameters param = new DynamicParameters();

                param.Add("Name", model.Name);
                param.Add("Address", model.Address);
                param.Add("City", model.City);
                param.Add("Country", model.Country);
                param.Add("PhoneNumber", model.PhoneNumber);
                param.Add("Email", model.Email);
                param.Add("CreateAt", DateTime.UtcNow);

                // When authentication is added 0 while be the UserId of the user who created the Hotel
                param.Add("CreateBy", 0);

                StringBuilder sql = new StringBuilder();

                // Create the insert query using a parametized query
                sql.Append(@"INSERT INTO Hotels
                    (
                        Name,
                        Address, 
                        City, 
                        Country, 
                        PhoneNumber, 
                        Email, 
                        CreateAt, 
                        CreateBy
                    ) 
                    Values 
                    (
                        @Name,
                        @Address, 
                        @City, 
                        @Country, 
                        @PhoneNumber, 
                        @Email, 
                        @CreateAt, 
                        @CreateBy
                    );  
                    SELECT CAST(SCOPE_IDENTITY() as int)");
                // The last query tells SQL Server to return id of the recently created Hotel

                var hotel_id = await _dataAccess.ReturnRowSql<int>(sql.ToString(), param);

                // Return that id from the method
                return hotel_id;

            }
            catch (Exception ex)
            {
                // log any errors
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(AddHotel)}");

                // rethrow the error up the chain
                throw new RepositoryException(
                  "Failed to create Hotel",
                  nameof(HotelRepository),
                  nameof(AddHotel),
                  "Create",
                  ex,
                  null);

            }
        }

        /// <summary>
        /// Delete a hotel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteHotel(int id)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(DeleteHotel)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Id", id);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"DELETE FROM Hotels WHERE Id = @Id");

                var deleteResult = await _dataAccess.ExecuteWithoutReturnSql(sql.ToString(), param);

                if (deleteResult > 0)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went" +
                    $" wrong in the HotelBooking.API.Data.HotelRepository {nameof(DeleteHotel)}");

                throw new RepositoryException(
                    "Failed to delete Hotel",
                    nameof(HotelRepository),
                    nameof(DeleteHotel),
                    "Delete",
                    ex,
                    null);
            }
        }

        /// <summary>
        /// Gets paginated list of hotels
        /// </summary>
        /// <param name="page">Current page (1-based indexing)</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>Paginated result containing hotels and metadata</returns>
        public async Task<PaginatedResult<Hotel>> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetAll)}");

                // Validate and normalize pagination parameters
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100); // Limiting maximum page size

                // Calculate pagination offset
                int offset = (page - 1) * pageSize;

                // Parameters for the query
                var parameters = new DynamicParameters();
                parameters.Add("@Offset", offset);
                parameters.Add("@PageSize", pageSize);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"SELECT Id, Name, Address, Country, PhoneNumber, Email, CreatedAt
                                FROM Hotels
                                ORDER BY Id
                                OFFSET @Offset ROWS
                                FETCH NEXT @PageSize ROWS ONLY; ");

                sql.Append(@"SELECT COUNT(Id) FROM Hotels;");

                // Using connection pooling implicitly through Dapper
                using var multiQuery = await _dataAccess.QueryMultipleAsync(
                    sql.ToString(),
                    parameters,
                    cancellationToken: cancellationToken);

                // Read both result sets
                var hotels = (await multiQuery.ReadAsync<Hotel>()).ToList();
                var totalCount = await multiQuery.ReadFirstOrDefaultAsync<int>();

                // Create paginated result with metadata
                return new PaginatedResult<Hotel>(
                    items: hotels,
                    totalCount: totalCount,
                    page: page,
                    pageSize: pageSize);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated hotels - Page: {Page}, PageSize: {PageSize}", page, pageSize);

                // Using a more specific exception type with structured information
                throw new RepositoryException(
                    message: "Failed to retrieve paginated hotels",
                    nameof(HotelRepository),
                   nameof(GetAll),
                    operation: "GetAllHotels",
                    innerException: ex);
            }
        }

        /// <summary>
        /// Get a hotel by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Hotel> GetHotelById(int id)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetHotelById)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Id", id);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"SELECT Id, Name, Address, Country, PhoneNumber, Email, CreateAt FROM Hotels WHERE Id = @Id");

                var res = await _dataAccess.ReturnRowSql<Hotel>(sql.ToString(), param);

                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(GetHotelById)}");

                throw new RepositoryException(
                    "Failed to get Hotel by Id",
                    nameof(HotelRepository),
                    nameof(GetHotelById),
                    "GetById",
                    ex,
                    null);
            }
        }

        /// <summary>
        /// Get a hotel by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Hotel> GetHotelByName(string name)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetHotelByName)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Name", name);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"SELECT Id, Name, Address, Country, PhoneNumber, Email, CreateAt FROM Hotels WHERE Name = @Name");

                var res = await _dataAccess.ReturnRowSql<Hotel>(sql.ToString(), param);

                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(GetHotelByName)}");

                throw new RepositoryException(
                    "Failed to get Hotel by Name",
                    nameof(HotelRepository),
                    nameof(GetHotelByName),
                    "GetByName",
                    ex,
                    null);
            }
        }

        /// <summary>
        /// Update a hotel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateHotel(HotelEntity model)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(UpdateHotel)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Id", model.Id);
                param.Add("Name", model.Name);
                param.Add("Address", model.Address);
                param.Add("City", model.City);
                param.Add("Country", model.Country);
                param.Add("PhoneNumber", model.PhoneNumber);
                param.Add("Email", model.Email);
                param.Add("UpdateAt", DateTime.UtcNow);

                // When authentication is added 0 while be the UserId of the user who created the Hotel
                param.Add("UpdateBy", 0);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"UPDATE [dbo].[Hotels]
                   SET [Address] = @Address
                      ,[Name] = @Name
                      ,[City] = @City
                      ,[Country] = @Country
                      ,[PhoneNumber] = @PhoneNumber
                      ,[Email] = @Email
                      ,[UpdateAt] = @UpdateAt
                      ,[UpdateBy] = @UpdateBy
                 WHERE Id = @Id");

                var updateResult = await _dataAccess.ExecuteWithoutReturnSql(sql.ToString(), param);

                if (updateResult > 0)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(UpdateHotel)}");

                throw new RepositoryException(
                    "Failed to update Hotel",
                    nameof(HotelRepository),
                    nameof(UpdateHotel),
                    "Update",
                    ex,
                    null);
            }
        }
    }
}
