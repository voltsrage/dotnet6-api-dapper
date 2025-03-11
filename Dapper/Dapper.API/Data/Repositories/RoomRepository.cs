using Dapper.API.Data.Dapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Rooms;
using Dapper.API.Entities;
using Dapper.API.Exceptions;
using Dapper.API.Helpers;
using Dapper.API.Models.Pagination;
using System.Data;

namespace Dapper.API.Data.Repositories
{
    /// <summary>
    /// Repository implementation for room data access
    /// </summary>
    public class RoomRepository : IRoomRepository
    {
        private readonly IDapperHandler _dapper;
        private readonly ILogger<RoomRepository> _logger;
        private const string REPOSITORY_NAME = nameof(RoomRepository);
        private readonly PaginationHelper _paginationHelper;

        public RoomRepository(
            IDapperHandler dapper,
            ILogger<RoomRepository> logger,
            PaginationHelper paginationHelper)
        {
            _dapper = dapper ?? throw new ArgumentNullException(nameof(dapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paginationHelper = paginationHelper;
        }

        /// <inheritdoc/>
        public async Task<Room> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                const string sql = @"
                SELECT 
                    r.Id, r.HotelId, r.RoomNumber, r.RoomTypeId, rt.Name as RoomTypeName,
                    r.PricePerNight, r.IsAvailable, r.MaxOccupancy, r.EntityStatusId, r.CreatedAt,
                    h.Name as HotelName
                FROM Rooms r
                INNER JOIN Hotels h ON r.HotelId = h.Id
                INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.Id
                WHERE r.Id = @Id AND r.EntityStatusId = 1";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return await _dapper.ReturnRowSql<Room>(sql, parameters, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room by ID {Id}", id);
                throw new RepositoryException(
                    $"Failed to get room with ID {id}",
                    REPOSITORY_NAME,
                    nameof(GetByIdAsync),
                    "GetById",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Room>> GetByHotelIdAsync(int hotelId, PaginationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create SQL builder for the query
                var queryBuilder = new QueryBuilder("Rooms", "r")
                    .Select(@"
                    r.Id, r.HotelId, r.RoomNumber, r.RoomTypeId, rt.Name as RoomTypeName,
                    r.PricePerNight, r.IsAvailable, r.MaxOccupancy, r.EntityStatusId, r.CreatedAt,
                    h.Name as HotelName")
                    .InnerJoin("Hotels", "h", "r.HotelId = h.Id")
                    .InnerJoin("RoomTypes", "rt", "r.RoomTypeId = rt.Id")
                    .Where("r.EntityStatusId = 1 AND r.HotelId = @HotelId")
                    .AddParameter("@HotelId", hotelId);

                // Define filterable columns
                var filterableColumns = new Dictionary<string, string>
            {
                { "roomTypeId", "r.RoomTypeId" },
                { "isAvailable", "r.IsAvailable" },
                { "priceMin", "r.PricePerNight" },
                { "priceMax", "r.PricePerNight" },
                { "maxOccupancy", "r.MaxOccupancy" }
            };

                // Define searchable columns
                var searchableColumns = new[] { "r.RoomNumber", "rt.Name", "h.Name", "rt.Name" };


                // Get paginated results
                return await _paginationHelper.GetPaginatedDataWithJoinsAsync<Room>(
                    request,
                    queryBuilder,
                    "Rooms",
                    filterableColumns,searchableColumns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rooms for hotel ID {HotelId}", hotelId);
                throw new RepositoryException(
                    $"Failed to get rooms for hotel with ID {hotelId}",
                    REPOSITORY_NAME,
                    nameof(GetByHotelIdAsync),
                    "GetByHotelId",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Room>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create SQL builder for the query
                var queryBuilder = new QueryBuilder("Rooms", "r")
                    .Select(@"
                    r.Id, r.HotelId, r.RoomNumber, r.RoomTypeId, rt.Name as RoomTypeName,
                    r.PricePerNight, r.IsAvailable, r.MaxOccupancy, r.EntityStatusId, r.CreatedAt,
                    h.Name as HotelName")
                    .InnerJoin("Hotels", "h", "r.HotelId = h.Id")
                    .InnerJoin("RoomTypes", "rt", "r.RoomTypeId = rt.Id")
                    .Where("r.EntityStatusId = 1");

                // Define filterable columns
                var filterableColumns = new Dictionary<string, string>
            {
                { "hotelId", "r.HotelId" },
                { "roomTypeId", "r.RoomTypeId" },
                { "isAvailable", "r.IsAvailable" },
                { "priceMin", "r.PricePerNight" },
                { "priceMax", "r.PricePerNight" },
                { "maxOccupancy", "r.MaxOccupancy" },
                { "country", "h.Country" },
                { "city", "h.City" }
            };

                // Define searchable columns
                var searchableColumns = new[] { "r.RoomNumber", "rt.Name", "h.Name", "h.City", "h.Country" };

                // Get paginated results
                return await _paginationHelper.GetPaginatedDataWithJoinsAsync<Room>(
                    request,
                    queryBuilder,
                    "Rooms",
                    filterableColumns, searchableColumns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");
                throw new RepositoryException(
                    "Failed to get all rooms",
                    REPOSITORY_NAME,
                    nameof(GetAllAsync),
                    "GetAll",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Room> CreateAsync(RoomEntity room, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creating new room in hotel {HotelId}", room.HotelId);

                // Check if hotel exists
                var hotelParameters = new DynamicParameters();
                hotelParameters.Add("@HotelId", room.HotelId);

                var hotelExists = await _dapper.ExecuteWithScalarSQL<int>(
                    "SELECT COUNT(1) FROM Hotels WHERE Id = @HotelId AND EntityStatusId = 1",
                    hotelParameters,
                    cancellationToken: cancellationToken);

                if (hotelExists == 0)
                {
                    throw new RepositoryException(
                        $"Hotel with ID {room.HotelId} not found or inactive",
                        REPOSITORY_NAME,
                        nameof(CreateAsync),
                        "Create",
                        null);
                }

                // Check for duplicate room number
                var duplicateParameters = new DynamicParameters();
                duplicateParameters.Add("@HotelId", room.HotelId);
                duplicateParameters.Add("@RoomNumber", room.RoomNumber);

                var roomExists = await _dapper.ExecuteWithScalarSQL<int>(
                    "SELECT COUNT(1) FROM Rooms WHERE HotelId = @HotelId AND RoomNumber = @RoomNumber AND EntityStatusId = 1",
                    duplicateParameters,
                    cancellationToken: cancellationToken);

                if (roomExists > 0)
                {
                    throw new RepositoryException(
                        $"Room number {room.RoomNumber} already exists in hotel {room.HotelId}",
                        REPOSITORY_NAME,
                        nameof(CreateAsync),
                        "Create",
                        null);
                }

                // Check if room type exists
                var roomTypeParameters = new DynamicParameters();
                roomTypeParameters.Add("@RoomTypeId", room.RoomTypeId);

                var roomTypeExists = await _dapper.ExecuteWithScalarSQL<int>(
                    "SELECT COUNT(1) FROM RoomTypes WHERE Id = @RoomTypeId",
                    roomTypeParameters,
                    cancellationToken: cancellationToken);

                if (roomTypeExists == 0)
                {
                    throw new RepositoryException(
                        $"Room type with ID {room.RoomTypeId} not found",
                        REPOSITORY_NAME,
                        nameof(CreateAsync),
                        "Create",
                        null);
                }

                // Create room
                var parameters = new DynamicParameters();
                parameters.Add("@HotelId", room.HotelId);
                parameters.Add("@RoomNumber", room.RoomNumber);
                parameters.Add("@RoomTypeId", room.RoomTypeId);
                parameters.Add("@PricePerNight", room.PricePerNight);
                parameters.Add("@IsAvailable", room.IsAvailable);
                parameters.Add("@MaxOccupancy", room.MaxOccupancy);
                parameters.Add("@EntityStatusId", 1); // Active
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@CreatedBy", userId);
                parameters.Add("@UpdatedBy", userId);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                const string insertSql = @"
                INSERT INTO Rooms (
                    HotelId, RoomNumber, RoomTypeId, PricePerNight, IsAvailable, MaxOccupancy, 
                    EntityStatusId, CreatedAt, CreatedBy, UpdatedBy
                ) 
                VALUES (
                    @HotelId, @RoomNumber, @RoomTypeId, @PricePerNight, @IsAvailable, @MaxOccupancy, 
                    @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedBy
                );
                SET @Id = SCOPE_IDENTITY();";

                await _dapper.ExecuteWithoutReturnSql(insertSql, parameters, cancellationToken: cancellationToken);

                int newId = parameters.Get<int>("@Id");

                // Retrieve the newly created room with full details
                return await GetByIdAsync(newId, cancellationToken);
            }
            catch (RepositoryException)
            {
                // Re-throw repository exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                throw new RepositoryException(
                    "Failed to create room",
                    REPOSITORY_NAME,
                    nameof(CreateAsync),
                    "Create",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Room> UpdateAsync(RoomEntity room, int userId, CancellationToken cancellationToken = default)
        {
            var id = room.Id;
            try
            {
                _logger.LogInformation("Updating room with ID {Id}", id);

                // Check if room exists
                var existingRoom = await GetByIdAsync(id, cancellationToken);
                if (existingRoom == null)
                {
                    throw new RepositoryException(
                        $"Room with ID {id} not found or inactive",
                        REPOSITORY_NAME,
                        nameof(UpdateAsync),
                        "Update",
                        null);
                }

                // Check if trying to change hotel
                if (existingRoom.HotelId != room.HotelId)
                {
                    throw new RepositoryException(
                        "Changing the hotel of an existing room is not allowed",
                        REPOSITORY_NAME,
                        nameof(UpdateAsync),
                        "Update",
                        null);
                }

                // Check for duplicate room number
                if (existingRoom.RoomNumber != room.RoomNumber)
                {
                    var duplicateParameters = new DynamicParameters();
                    duplicateParameters.Add("@HotelId", room.HotelId);
                    duplicateParameters.Add("@RoomNumber", room.RoomNumber);
                    duplicateParameters.Add("@Id", id);

                    var roomExists = await _dapper.ExecuteWithScalarSQL<int>(
                        "SELECT COUNT(1) FROM Rooms WHERE HotelId = @HotelId AND RoomNumber = @RoomNumber AND Id != @Id AND EntityStatusId = 1",
                        duplicateParameters,
                        cancellationToken: cancellationToken);

                    if (roomExists > 0)
                    {
                        throw new RepositoryException(
                            $"Room number {room.RoomNumber} already exists in hotel {room.HotelId}",
                            REPOSITORY_NAME,
                            nameof(UpdateAsync),
                            "Update",
                            null);
                    }
                }

                // Check if room type exists
                var roomTypeParameters = new DynamicParameters();
                roomTypeParameters.Add("@RoomTypeId", room.RoomTypeId);

                var roomTypeExists = await _dapper.ExecuteWithScalarSQL<int>(
                    "SELECT COUNT(1) FROM RoomTypes WHERE Id = @RoomTypeId",
                    roomTypeParameters,
                    cancellationToken: cancellationToken);

                if (roomTypeExists == 0)
                {
                    throw new RepositoryException(
                        $"Room type with ID {room.RoomTypeId} not found",
                        REPOSITORY_NAME,
                        nameof(UpdateAsync),
                        "Update",
                        null);
                }

                // Update room
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@RoomNumber", room.RoomNumber);
                parameters.Add("@RoomTypeId", room.RoomTypeId);
                parameters.Add("@PricePerNight", room.PricePerNight);
                parameters.Add("@IsAvailable", room.IsAvailable);
                parameters.Add("@MaxOccupancy", room.MaxOccupancy);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedBy", userId);

                const string updateSql = @"
                UPDATE Rooms 
                SET RoomNumber = @RoomNumber,
                    RoomTypeId = @RoomTypeId,
                    PricePerNight = @PricePerNight,
                    IsAvailable = @IsAvailable,
                    MaxOccupancy = @MaxOccupancy,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id AND EntityStatusId = 1";

                int rowsAffected = await _dapper.ExecuteWithoutReturnSql(updateSql, parameters, cancellationToken: cancellationToken);

                if (rowsAffected == 0)
                {
                    throw new RepositoryException(
                        $"Failed to update room with ID {id} - not found or already deleted",
                        REPOSITORY_NAME,
                        nameof(UpdateAsync),
                        "Update",
                        null);
                }

                // Retrieve the updated room with full details
                return await GetByIdAsync(id, cancellationToken);
            }
            catch (RepositoryException)
            {
                // Re-throw repository exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room with ID {Id}", id);
                throw new RepositoryException(
                    $"Failed to update room with ID {id}",
                    REPOSITORY_NAME,
                    nameof(UpdateAsync),
                    "Update",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting room with ID {Id}", id);

                // Soft delete - update status to DeletedForEveryone (3)
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedBy", userId);
                parameters.Add("@EntityStatusId", 3); // DeletedForEveryone

                const string updateSql = @"
                UPDATE Rooms 
                SET EntityStatusId = @EntityStatusId,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id AND EntityStatusId = 1";

                int rowsAffected = await _dapper.ExecuteWithoutReturnSql(updateSql, parameters, cancellationToken: cancellationToken);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room with ID {Id}", id);
                throw new RepositoryException(
                    $"Failed to delete room with ID {id}",
                    REPOSITORY_NAME,
                    nameof(DeleteAsync),
                    "Delete",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating availability for room ID {Id} to {IsAvailable}", id, isAvailable);

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@IsAvailable", isAvailable);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedBy", userId);

                const string updateSql = @"
                UPDATE Rooms 
                SET IsAvailable = @IsAvailable,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id AND EntityStatusId = 1";

                int rowsAffected = await _dapper.ExecuteWithoutReturnSql(updateSql, parameters, cancellationToken: cancellationToken);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability for room ID {Id}", id);
                throw new RepositoryException(
                    $"Failed to update availability for room with ID {id}",
                    REPOSITORY_NAME,
                    nameof(UpdateAvailabilityAsync),
                    "UpdateAvailability",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RoomType>> GetRoomTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                const string sql = "SELECT Id, Name, Description FROM RoomTypes ORDER BY Id";
                return await _dapper.ReturnListSql<RoomType>(sql, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room types");
                throw new RepositoryException(
                    "Failed to get room types",
                    REPOSITORY_NAME,
                    nameof(GetRoomTypesAsync),
                    "GetRoomTypes",
                    ex);
            }
        }
    }
}
