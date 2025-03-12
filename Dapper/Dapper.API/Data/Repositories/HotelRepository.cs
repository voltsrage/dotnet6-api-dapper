using Dapper.API.Data.Dapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Dtos.Rooms;
using Dapper.API.Entities;
using Dapper.API.Enums.StandardEnums;
using Dapper.API.Exceptions;
using Dapper.API.Helpers;
using Dapper.API.Models;
using Dapper.API.Models.Pagination;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Transactions;

namespace Dapper.API.Data.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly IDapperHandler _dataAccess;
        private readonly ILogger<HotelRepository> _logger;
        private readonly IPaginationHelper _paginationHelper;
        private const string REPOSITORY_NAME = nameof(HotelRepository);

        public HotelRepository(ILogger<HotelRepository> logger, IDapperHandler dataAccess, IPaginationHelper paginationHelper)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _paginationHelper = paginationHelper;
        }

        /// <inheritdoc/>
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
                param.Add("CreatedAt", DateTime.UtcNow);

                // When authentication is added 0 while be the UserId of the user who created the Hotel
                param.Add("CreatedBy", 0);

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
                        CreatedAt, 
                        CreatedBy
                    ) 
                    Values 
                    (
                        @Name,
                        @Address, 
                        @City, 
                        @Country, 
                        @PhoneNumber, 
                        @Email, 
                        @CreatedAt, 
                        @CreatedBy
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
                  REPOSITORY_NAME,
                  nameof(AddHotel),
                  "Create",
                  ex,
                  null);

            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<HotelEntity>> CreateManyAsync(IEnumerable<HotelEntity> hotels, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating {Count} hotels in repository", hotels.Count());
                var createdHotels = new List<HotelEntity>();

                await _dataAccess.ExecuteInTransaction(async (connection, transaction, token) =>
                {
                    foreach(var hotel in hotels)
                    {
                        // Set Id parameter for output
                        var parameters = new DynamicParameters();
                        parameters.Add("@Name", hotel.Name);
                        parameters.Add("@Address", hotel.Address);
                        parameters.Add("@City", hotel.City);
                        parameters.Add("@Country", hotel.Country);
                        parameters.Add("@PhoneNumber", hotel.PhoneNumber);
                        parameters.Add("@Email", hotel.Email);
                        parameters.Add("@CreatedAt", DateTime.UtcNow);
                        parameters.Add("@CreatedBy",0);

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
                            CreatedAt, 
                            CreatedBy
                        ) 
                        Values 
                        (
                            @Name,
                            @Address, 
                            @City, 
                            @Country, 
                            @PhoneNumber, 
                            @Email, 
                            @CreatedAt, 
                            @CreatedBy
                        );  
                        SELECT CAST(SCOPE_IDENTITY() as int)");
                        // The last query tells SQL Server to return id of the recently created Hotel

                        var hotel_id = await _dataAccess.ReturnRowSqlInTransaction<int>(sql.ToString(),transaction, parameters, token);

                        hotel.Id = hotel_id;

                        createdHotels.Add(hotel);

                        _logger.LogDebug("Created hotel with ID: {Id}", hotel.Id);
                    }
                });

                _logger.LogInformation("Successfully created {Count} hotels", createdHotels.Count);

                return createdHotels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotels batch");
                throw new RepositoryException(
                    "Failed to create hotels batch",
                    REPOSITORY_NAME,
                    nameof(CreateManyAsync),
                    "CreateMany",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteHotel(int id)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(DeleteHotel)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Id", id);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"UPDATE Hotels SET EntityStatusId = 3 WHERE Id = @Id");

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
                    REPOSITORY_NAME,
                    nameof(DeleteHotel),
                    "Delete",
                    ex,
                    null);
            }
        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Hotel>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default)
        {
          
            _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetAll)}");

            // Define the columns to retrieve
            string columns = "Id, Name, Address, City, Country, PhoneNumber, Email, EntityStatusId, CreatedAt";

            // Define which columns to search
            string[] searchableColumns = { "Name", "Address", "City", "Country", "Email" };

            // Define base condition (only active hotels)
            string baseCondition = "EntityStatusId = 1";

            return await _paginationHelper.GetPaginatedResultAsync<Hotel>(
                pagination,
                tableName: "Hotels",
                columns: columns,
                searchableColumns: searchableColumns,
                baseCondition: baseCondition,
                sortColumn: "Id",
                cancellationToken: cancellationToken);         
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Hotel>> GetByIdsAsync(int[] ids, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Processing {nameof(GetByIdsAsync)} in {REPOSITORY_NAME}");

                if (ids == null || !ids.Any())
                    return Enumerable.Empty<Hotel>();

                // Create the parameters for the IN clause
                var parameters = new DynamicParameters();

                parameters.Add("@Ids", ids);

                // Build a SQL query with an IN clause for the IDs
                var sql = new StringBuilder();
                sql.Append("SELECT Id, Name, Address, City, Country, PhoneNumber, Email, EntityStatusId, CreatedAt ");
                sql.Append("FROM Hotels ");
                sql.Append($"WHERE Id IN @Ids");

                // Execute the query
                var hotels = await _dataAccess.ReturnListSql<Hotel>(
                    sql.ToString(),
                    parameters,
                    cancellationToken: cancellationToken);

                return hotels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hotels by IDs");
                throw new RepositoryException(
                    "Failed to retrieve hotels by IDs",
                    REPOSITORY_NAME,
                    nameof(GetByIdsAsync),
                    "GetByIds",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Hotel> GetHotelById(int id)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetHotelById)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Id", id);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"SELECT Id, Name, Address, City, Country, PhoneNumber, Email, CreatedAt, EntityStatusId FROM Hotels WHERE Id = @Id");

                var res = await _dataAccess.ReturnRowSql<Hotel>(sql.ToString(), param);

                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(GetHotelById)}");

                throw new RepositoryException(
                    "Failed to get Hotel by Id",
                    REPOSITORY_NAME,
                    nameof(GetHotelById),
                    "GetById",
                    ex,
                    null);
            }
        }

        /// <inheritdoc/>
        public async Task<Hotel> GetHotelByName(string name)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetHotelByName)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Name", name);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"SELECT Id, Name, Address, City, Country, PhoneNumber, Email, CreatedAt, EntityStatusId FROM Hotels WHERE Name = @Name");

                var res = await _dataAccess.ReturnRowSql<Hotel>(sql.ToString(), param);

                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(GetHotelByName)}");

                throw new RepositoryException(
                    "Failed to get Hotel by Name",
                    REPOSITORY_NAME,
                    nameof(GetHotelByName),
                    "GetByName",
                    ex,
                    null);
            }
        }

        /// <inheritdoc/>
        public async Task<Hotel> GetHotelByNameAndAddress(string name, string address)
        {
            try
            {
                _logger.LogInformation($"Processing HotelBooking.API.Data.HotelRepository {nameof(GetHotelByNameAndAddress)}");

                DynamicParameters param = new DynamicParameters();

                param.Add("Name", name);
                param.Add("Address", address);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"SELECT Id, Name, Address, City, Country, PhoneNumber, Email, CreatedAt, EntityStatusId FROM Hotels WHERE Name = @Name AND Address = @Address AND EntityStatusId = 1");

                var res = await _dataAccess.ReturnRowSql<Hotel>(sql.ToString(), param);

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the HotelBooking.API.Data.HotelRepository {nameof(GetHotelByNameAndAddress)}");
                throw new RepositoryException(
                    "Failed to get Hotel by Name and Address",
                    REPOSITORY_NAME,
                    nameof(GetHotelByNameAndAddress),
                    "GetByNameAndAddress",
                    ex,
                    null);
            }
        }

        /// <inheritdoc/>
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
                param.Add("UpdatedAt", DateTime.UtcNow);

                // When authentication is added 0 while be the UserId of the user who created the Hotel
                param.Add("UpdatedBy", 0);

                StringBuilder sql = new StringBuilder();

                sql.Append(@"UPDATE [dbo].[Hotels]
                   SET [Address] = @Address
                      ,[Name] = @Name
                      ,[City] = @City
                      ,[Country] = @Country
                      ,[PhoneNumber] = @PhoneNumber
                      ,[Email] = @Email
                      ,[UpdatedAt] = @UpdatedAt
                      ,[UpdatedBy] = @UpdatedBy
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
                    REPOSITORY_NAME,
                    nameof(UpdateHotel),
                    "Update",
                    ex,
                    null);
            }
        }

        /// <inheritdoc/>
        public async Task<BulkDeleteResult> DeleteManyAsync(IEnumerable<int> ids, int userId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting {Count} hotels in repository", ids.Count());

                var result = new BulkDeleteResult
                {
                    SuccessfullyDeletedIds = new List<int>(),
                    NotFoundIds = new List<int>(),
                    FailedIds = new Dictionary<int, string>()
                };

                // First verify which hotels exist
                var existingHotels = await GetByIdsAsync(ids.ToArray(), cancellationToken);
                var existingIds = existingHotels.Select(h => h.Id).ToHashSet();

                // Identify not found IDs
                foreach (var id in ids)
                {
                    if (!existingIds.Contains(id))
                    {
                        result.NotFoundIds.Add(id);
                    }
                }

                // Process deletions for existing hotels
                var idsToDelete = ids.Where(id => existingIds.Contains(id)).ToList();

                if (idsToDelete.Any())
                {
                    // Execute all deletions in a single transaction
                    await _dataAccess.ExecuteInTransaction(async (connection, transaction, token) =>
                    {
                        foreach (var id in idsToDelete)
                        {
                            try
                            {
                                // Instead of hard-deleting, we'll set the status to 'Deleted'
                                // This is a common pattern for maintaining audit history
                                var parameters = new DynamicParameters();
                                parameters.Add("@Id", id);
                                parameters.Add("@UpdatedBy", userId);
                                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                                parameters.Add("@EntityStatusId", 3); // 3 = DeletedForEveryone

                                const string updateSql = @"
                            UPDATE Hotels 
                            SET EntityStatusId = @EntityStatusId,
                                UpdatedBy = @UpdatedBy,
                                UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";

                                int rowsAffected = await _dataAccess.ExecuteWithoutReturnSqlInTransaction(
                                    updateSql,
                                    transaction,
                                    parameters,
                                    token);

                                if (rowsAffected > 0)
                                {
                                    result.SuccessfullyDeletedIds.Add(id);
                                    _logger.LogDebug("Marked hotel with ID {Id} as deleted", id);
                                }
                                else
                                {
                                    // This shouldn't happen since we verified existence, but handle it just in case
                                    result.FailedIds.Add(id, "Failed to delete. Hotel not found or already deleted.");
                                    _logger.LogWarning("Failed to delete hotel with ID {Id} - not found or already deleted", id);
                                }
                            }
                            catch (Exception ex)
                            {
                                result.FailedIds.Add(id, "Error deleting hotel: " + ex.Message);
                                _logger.LogError(ex, "Error deleting hotel with ID {Id}", id);
                                throw; // This will trigger a transaction rollback
                            }
                        }
                    }, cancellationToken: cancellationToken);
                }

                _logger.LogInformation("Bulk delete complete. Deleted: {SuccessCount}, Not found: {NotFoundCount}, Failed: {FailedCount}",
                    result.SuccessfullyDeletedIds.Count, result.NotFoundIds.Count, result.FailedIds.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hotels batch");
                throw new RepositoryException(
                    "Failed to delete hotels batch",
                    REPOSITORY_NAME,
                    nameof(DeleteManyAsync),
                    "DeleteMany",
                    ex);
            }
        }
            
        /// <inheritdoc/>
        public async Task<PaginatedResult<HotelWithRooms>> GetHotelsWithRoomsAsync(PaginationRequest pagination, CancellationToken cancellationToken = default)
        {
            try
            {
                // Define the columns to retrieve
                string columns = "Id, Name, Address, City, Country, PhoneNumber, Email, EntityStatusId, CreatedAt";

                // Define which columns to search
                string[] searchableColumns = { "Name", "Address", "City", "Country", "Email" };

                // Define base condition (only active hotels)
                string baseCondition = "EntityStatusId = 1";

                var hotelsResult =  await _paginationHelper.GetPaginatedResultAsync<Hotel>(
                    pagination,
                    tableName: "Hotels",
                    columns: columns,
                    searchableColumns: searchableColumns,
                    baseCondition: baseCondition,
                    sortColumn: "Id",
                    cancellationToken: cancellationToken);

                // Initialize and empty hotelroom result
                var result = PaginatedResult<HotelWithRooms>.Empty(pagination.Page, pagination.PageSize);                

                // For empty result, return early
                if (hotelsResult.Items == null || !hotelsResult.Items.Any())
                {
                    return result;
                }

                var hotelIds = hotelsResult.Items.Select(x => x.Id).ToArray();

                // Get rooms for all hotels

                var roomsQuery = new StringBuilder();

                roomsQuery.Append(@"SELECT
                            r.Id, r.HotelId, r.RoomNumber, 
                            r.RoomTypeId, rt.Name as RoomTypeName, r.PricePerNight, r.CreatedAt, 
                            r.CreatedBy, r.UpdatedAt, 
                            r.UpdatedBy, r.EntityStatusId 
                        FROM Rooms r
                        INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.Id
                        WHERE r.EntityStatusId = 1 AND r.HotelId IN @HotelIds ");

                DynamicParameters parameters = new();

                parameters.Add("@HotelIds", hotelIds);

                var rooms = await _dataAccess.ReturnListSql<Room>(roomsQuery.ToString(), parameters);

                // Group rooms by hotel ID
                var roomsByHotel = rooms.GroupBy(r => r.HotelId)
                                       .ToDictionary(g => g.Key, g => g.ToList());

                foreach(var hotel in hotelsResult.Items)
                {
                    result.Items.Add(new HotelWithRooms
                    {
                        Id = hotel.Id,
                        Name = hotel.Name,
                        Address = hotel.Address,
                        City = hotel.City,
                        Country = hotel.Country,
                        PhoneNumber = hotel.PhoneNumber,
                        Email = hotel.Email,
                        CreateAt = hotel.CreatedAt ?? DateTime.Now,
                        Rooms = roomsByHotel.TryGetValue(hotel.Id, out var hotelRooms)
                                    ? hotelRooms
                                    : new List<Room>()
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hotels with rooms");
                throw new RepositoryException(
                    "Failed to get hotels with rooms",
                    REPOSITORY_NAME,
                    nameof(GetHotelsWithRoomsAsync),
                    "GetHotelsWithRooms",
                    ex);
            }

        }

        /// <inheritdoc/>
        public async Task<HotelWithRooms> GetHotelWithRoomsByIdAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            try
            {
                DynamicParameters parameters = new();

                parameters.Add("@HotelId", hotelId);

                // Get hotel data
                var hotelQuery = @"
                        SELECT Id, Name, Address, City, Country, PhoneNumber, Email, EntityStatusId, CreatedAt
                        FROM Hotels
                        WHERE Id = @HotelId AND EntityStatusId = 1";


                var hotel = await _dataAccess.ReturnRowSql<Hotel>(hotelQuery, parameters);

                if (hotel == null)
                {
                    return null;
                }

                var roomsQuery = new StringBuilder();

                roomsQuery.Append(@"SELECT
                            r.Id, r.HotelId, r.RoomNumber, 
                            r.RoomTypeId, rt.Name as RoomTypeName, r.PricePerNight, r.CreatedAt, 
                            r.CreatedBy, r.UpdatedAt, 
                            r.UpdatedBy, r.EntityStatusId 
                        FROM Rooms r
                        INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.Id
                        WHERE r.EntityStatusId = 1 AND r.HotelId =  @HotelId ");

                var rooms = await _dataAccess.ReturnListSql<Room>(roomsQuery.ToString(), parameters);

                return new HotelWithRooms
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    City = hotel.City,
                    Country = hotel.Country,
                    PhoneNumber = hotel.PhoneNumber,
                    Email = hotel.Email,
                    CreateAt = hotel.CreatedAt ?? DateTime.Now,
                    Rooms = rooms.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hotel with rooms by ID: {HotelId}", hotelId);
                throw new RepositoryException(
                    $"Failed to get hotel with rooms (ID: {hotelId})",
                    REPOSITORY_NAME,
                    nameof(GetHotelWithRoomsByIdAsync),
                    "GetHotelWithRoomsById",
                    ex);
            }
        }
    }
}
