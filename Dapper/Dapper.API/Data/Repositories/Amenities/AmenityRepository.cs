using Dapper.API.Data.Dapper;
using Dapper.API.Data.Repositories.Interfaces.Amenities;
using Dapper.API.Entities.Amentities;
using Dapper.API.Enums.StandardEnums;
using Dapper.API.Exceptions;
using Dapper.API.Models;
using System.Data;
using System.Text;

namespace Dapper.API.Data.Repositories.Amenities
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly IDapperHandler _dataAccess;
        private readonly ILogger<AmenityRepository> _logger;
        private const string REPOSITORY_NAME = nameof(AmenityRepository);

        public AmenityRepository(
            ILogger<AmenityRepository> logger, 
            IDapperHandler dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        /// <inheritdoc/>
        public async Task<int> AddAmenityAsync(BaseAmenity amenity)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(AddAmenityAsync)}");


                await _dataAccess.ExecuteInTransaction(async (connection, transaction, token) =>
                {
                    // Get or create the amenity type ID
                    int amenityTypeId = await GetOrCreateAmenityTypeId(transaction, GetAmenityTypeName(amenity));

                    // Add the base amenity
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Name", amenity.Name);
                    param.Add("Description", amenity.Description);
                    param.Add("PriceModifier", amenity.PriceModifier);
                    param.Add("IsStandard", amenity.IsStandard);
                    param.Add("AmenityTypeId", amenityTypeId);
                    param.Add("InternalIdentifier", amenity.InternalIdentifier);
                    param.Add("EntityStatusId", 1); // Active
                    param.Add("CreatedAt", DateTime.UtcNow);
                    param.Add("CreatedBy", amenity.CreatedBy);
                    param.Add("UpdatedAt", DateTime.UtcNow);
                    param.Add("UpdatedBy", amenity.UpdatedBy);

                    StringBuilder sql = new StringBuilder();
                    sql.Append(@"
                        INSERT INTO Amenities 
                            (Name, Description, PriceModifier, IsStandard, AmenityTypeId, 
                             InternalIdentifier, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                        VALUES 
                            (@Name, @Description, @PriceModifier, @IsStandard, @AmenityTypeId, 
                             @InternalIdentifier, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                        SELECT CAST(SCOPE_IDENTITY() as int)");

                    int amenityId = await _dataAccess.ReturnRowSqlInTransaction<int>(
                        sql.ToString(), transaction, param, token);

                    // Now add type-specific details
                    if (amenity is WIFIAmenity wifiAmenity)
                    {
                        await AddWifiAmenityDetails(transaction, amenityId, wifiAmenity, token);
                    }
                    else if (amenity is MiniBarAmenity miniBarAmenity)
                    {
                        await AddMiniBarAmenityDetails(transaction, amenityId, miniBarAmenity, token);
                    }
                    else if (amenity is RoomServiceAmenity roomServiceAmenity)
                    {
                        await AddRoomServiceAmenityDetails(transaction, amenityId, roomServiceAmenity, token);
                    }

                    // Set the ID for the amenity object
                    amenity.Id = amenityId;
                });

                return amenity.Id;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(AddAmenityAsync)}");
                throw new RepositoryException(
                    "Failed to add amenity",
                    REPOSITORY_NAME,
                    nameof(AddAmenityAsync),
                    "Add",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> AddPremiumDecoratorAsync(PremiumAmenityDecorator decorator)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(AddPremiumDecoratorAsync)}");

                // Get base amenity's internal identifier
                DynamicParameters getIdParam = new DynamicParameters();
                getIdParam.Add("Id", decorator.Id);

                string internalIdQuery = "SELECT InternalIdentifier FROM Amenities WHERE Id = @Id AND EntityStatusId = 1";
                string baseAmenityInternalId = await _dataAccess.ReturnRowSql<string>(internalIdQuery, getIdParam);

                // Insert the decorator
                DynamicParameters param = new DynamicParameters();
                param.Add("BaseAmenityId", decorator.Id);
                param.Add("BaseAmenityInternalId", baseAmenityInternalId);
                param.Add("PremiumFeature", decorator.PremiumFeature);
                param.Add("AdditionalCost", decorator.AdditionalCost);
                param.Add("InternalIdentifier", decorator.InternalIdentifier);
                param.Add("EntityStatusId", 1); // Active
                param.Add("CreatedAt", DateTime.UtcNow);
                param.Add("CreatedBy", 0); // Add when authentication added
                param.Add("UpdatedAt", DateTime.UtcNow);
                param.Add("UpdatedBy", 0); // Add when authentication added

                StringBuilder sql = new StringBuilder();
                sql.Append(@"
                    INSERT INTO PremiumAmenityDecorators 
                        (BaseAmenityId, BaseAmenityInternalId, PremiumFeature, AdditionalCost, 
                         InternalIdentifier, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                    VALUES 
                        (@BaseAmenityId, @BaseAmenityInternalId, @PremiumFeature, @AdditionalCost, 
                         @InternalIdentifier, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                    SELECT CAST(SCOPE_IDENTITY() as int)");

                int decoratorId = await _dataAccess.ReturnRowSql<int>(sql.ToString(), param);

                return decoratorId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(AddPremiumDecoratorAsync)}");
                throw new RepositoryException(
                    "Failed to add premium decorator",
                    REPOSITORY_NAME,
                    nameof(AddPremiumDecoratorAsync),
                    "AddPremiumDecorator",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> AddSeasonalDecoratorAsync(SeasonalAmenityDecorator decorator)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(AddSeasonalDecoratorAsync)}");

                // Get base amenity's internal identifier
                DynamicParameters getIdParam = new DynamicParameters();
                getIdParam.Add("Id", decorator.Id);

                string internalIdQuery = "SELECT InternalIdentifier FROM Amenities WHERE Id = @Id AND EntityStatusId = 1";
                string baseAmenityInternalId = await _dataAccess.ReturnRowSql<string>(internalIdQuery, getIdParam);

                // Insert the decorator
                DynamicParameters param = new DynamicParameters();
                param.Add("BaseAmenityId", decorator.Id);
                param.Add("BaseAmenityInternalId", baseAmenityInternalId);
                param.Add("Season", decorator.Season);
                param.Add("StartDate", decorator.StartDate);
                param.Add("EndDate", decorator.EndDate);
                param.Add("SeasonalPriceAdjustment", decorator.SeasonalPriceAdjustment);
                param.Add("InternalIdentifier", decorator.InternalIdentifier);
                param.Add("EntityStatusId", 1); // Active
                param.Add("CreatedAt", DateTime.UtcNow);
                param.Add("CreatedBy", 0); // Add when authentication added
                param.Add("UpdatedAt", DateTime.UtcNow);
                param.Add("UpdatedBy", 0); // Add when authentication added

                StringBuilder sql = new StringBuilder();
                sql.Append(@"
                    INSERT INTO SeasonalAmenityDecorators 
                        (BaseAmenityId, BaseAmenityInternalId, Season, StartDate, EndDate, SeasonalPriceAdjustment, 
                         InternalIdentifier, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                    VALUES 
                        (@BaseAmenityId, @BaseAmenityInternalId, @Season, @StartDate, @EndDate, @SeasonalPriceAdjustment, 
                         @InternalIdentifier, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                    SELECT CAST(SCOPE_IDENTITY() as int)");

                int decoratorId = await _dataAccess.ReturnRowSql<int>(sql.ToString(), param);

                return decoratorId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(AddSeasonalDecoratorAsync)}");
                throw new RepositoryException(
                    "Failed to add seasonal decorator",
                    REPOSITORY_NAME,
                    nameof(AddSeasonalDecoratorAsync),
                    "AddSeasonalDecorator",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAmenityAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(DeleteAmenityAsync)}");

                DynamicParameters param = new DynamicParameters();
                param.Add("Id", id);
                param.Add("UpdatedAt", DateTime.UtcNow);
                param.Add("UpdatedBy", 0); // Replace with actual user ID when available
                param.Add("EntityStatusId", (int)EntityStatusEnum.DeletedForEveryone); // DeletedForEveryone

                StringBuilder sql = new StringBuilder();
                sql.Append(@"
                    UPDATE Amenities 
                    SET EntityStatusId = @EntityStatusId,
                        UpdatedAt = @UpdatedAt,
                        UpdatedBy = @UpdatedBy
                    WHERE Id = @Id");

                await _dataAccess.ExecuteWithoutReturnSql(sql.ToString(), param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(DeleteAmenityAsync)}");
                throw new RepositoryException(
                    "Failed to delete amenity",
                    REPOSITORY_NAME,
                    nameof(DeleteAmenityAsync),
                    "Delete",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BaseAmenity>> GetAllAmenitiesAsync()
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetAllAmenitiesAsync)}");

                var query = new StringBuilder();

                query.Append( @"
                    SELECT a.*, at.AmenityType 
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    WHERE a.EntityStatusId = 1");

                var amenities = new List<BaseAmenity>();

                // First get all base amenities with their types
                var results = await _dataAccess.ReturnListSql<dynamic>(query.ToString());

                // Process the result to create the appropriate amenity object
                foreach(var result in results)
                {
                    BaseAmenity amenity = await CreateAmenityFromDbResult(result);

                    if (amenity != null)
                    {
                        amenities.Add(amenity);
                    }
                }

                return amenities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetAllAmenitiesAsync)}");
                throw new RepositoryException(
                    "Failed to get all amenities",
                    REPOSITORY_NAME,
                    nameof(GetAllAmenitiesAsync),
                    "GetAll",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MiniBarAmenity>> GetAllMiniBarAmenitiesAsync()
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetAllMiniBarAmenitiesAsync)}");

                var query = $@"
                    SELECT a.*, at.AmenityType, mb.Id as MiniBarId, mb.IsComplimentary
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    INNER JOIN MiniBarAmenities mb ON a.Id = mb.AmenityId
                    WHERE at.AmenityType = {AmenityTypeLookup.minibar} AND a.EntityStatusId = 1 AND mb.EntityStatusId = 1";

                var results = await _dataAccess.ReturnListSql<dynamic>(query);
                var amenities = new List<MiniBarAmenity>();

                foreach (var result in results)
                {
                    // Get items for this minibar
                    DynamicParameters param = new DynamicParameters();
                    param.Add("MiniBarAmenityId", result.MiniBarId);

                    var itemsQuery = @"
                        SELECT Item 
                        FROM MiniBarItems 
                        WHERE MiniBarAmenityId = @MiniBarAmenityId AND EntityStatusId = 1";

                    var items = await _dataAccess.ReturnListSql<string>(itemsQuery, param);

                    var amenity = new MiniBarAmenity
                    (
                    result.Name,
                    result.Description,
                    result.PriceModifier,
                    result.IsStandard,
                    result.IsComplimentary,
                    items.ToList()
                    );

                    amenities.Add(amenity);
                }

                return amenities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetAllMiniBarAmenitiesAsync)}");
                throw new RepositoryException(
                    "Failed to get all MiniBar amenities",
                    REPOSITORY_NAME,
                    nameof(GetAllMiniBarAmenitiesAsync),
                    "GetAllMiniBar",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RoomServiceAmenity>> GetAllRoomServiceAmenitiesAsync()
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetAllRoomServiceAmenitiesAsync)}");

                var query = $@"
                    SELECT a.*, at.AmenityType, rs.HoursAvailable, rs.Is24Hours
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    INNER JOIN RoomServiceAmenities rs ON a.Id = rs.AmenityId
                    WHERE at.AmenityType = {AmenityTypeLookup.roomservice} AND a.EntityStatusId = 1 AND rs.EntityStatusId = 1";

                var results = await _dataAccess.ReturnListSql<dynamic>(query);
                var amenities = new List<RoomServiceAmenity>();

                foreach (var result in results)
                {
                    var amenity = new RoomServiceAmenity(
                        result.Name, 
                        result.Description, 
                        result.PriceModifier, 
                        result.IsStandard, 
                        result.HoursAvailable,
                        result.Is24Hours
                    );

                    amenities.Add(amenity);
                }

                return amenities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetAllRoomServiceAmenitiesAsync)}");
                throw new RepositoryException(
                    "Failed to get all RoomService amenities",
                    REPOSITORY_NAME,
                    nameof(GetAllRoomServiceAmenitiesAsync),
                    "GetAllRoomService",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WIFIAmenity>> GetAllWifiAmenitiesAsync()
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetAllWifiAmenitiesAsync)}");

                var query = $@"
                    SELECT a.*, at.AmenityType, w.NetworkName, w.Password, w.SpeedMbps
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    INNER JOIN WifiAmenities w ON a.Id = w.AmenityId
                    WHERE at.AmenityType = {AmenityTypeLookup.wifi} AND a.EntityStatusId = 1 AND w.EntityStatusId = 1";

                var results = await _dataAccess.ReturnListSql<dynamic>(query);
                var amenities = new List<WIFIAmenity>();

                foreach (var result in results)
                {
                    var amenity = new WIFIAmenity(
                    result.Name,
                    result.Description,
                    result.PriceModifier,
                    result.IsStandard,
                    result.NetworkName,
                    result.Password,
                    result.SpeedMbps);

                    amenities.Add(amenity);
                }

                return amenities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetAllWifiAmenitiesAsync)}");
                throw new RepositoryException(
                    "Failed to get all WiFi amenities",
                    REPOSITORY_NAME,
                    nameof(GetAllWifiAmenitiesAsync),
                    "GetAllWifi",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BaseAmenity> GetAmenityByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetAmenityByIdAsync)}");

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@Id", id);
                parameters.Add("@EntityStatusId", (int)EntityStatusEnum.Active);

                var query = new StringBuilder();

                query.Append(@"
                    SELECT a.*, at.AmenityType 
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    WHERE a.Id = @Id AND a.EntityStatusId = @EntityStatusId");

                var result = await _dataAccess.ReturnRowSql<dynamic>(query.ToString(), parameters);

                if(result == null)
                {
                    return null;
                }

                return await CreateAmenityFromDbResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetAmenityByIdAsync)}");
                throw new RepositoryException(
                    "Failed to get amenity by id",
                    REPOSITORY_NAME,
                    nameof(GetAmenityByIdAsync),
                    "GetById",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BaseAmenity> GetAmenityByNameAsync(string name)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetAmenityByNameAsync)}");

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@Name", name);
                parameters.Add("@EntityStatusId", (int)EntityStatusEnum.Active);

                var query = new StringBuilder();

                query.Append(@"
                    SELECT a.*, at.AmenityType 
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    WHERE a.Name = @Name and a.EntityStatusId = @EntityStatusId");

                var result = await _dataAccess.ReturnRowSql<dynamic>(query.ToString(), parameters);

                if (result == null)
                {
                    return null;
                }

                return await CreateAmenityFromDbResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetAmenityByNameAsync)}");
                throw new RepositoryException(
                    "Failed to get amenity by name",
                    REPOSITORY_NAME,
                    nameof(GetAmenityByNameAsync),
                    "GetByName",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IAmenity> GetDecoratedAmenityAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetDecoratedAmenityAsync)}");

                // First get the base amenity
                var baseAmenity = await GetAmenityByIdAsync(id);

                if (baseAmenity == null)
                    return null;

                IAmenity decoratedAmenity = baseAmenity;

                // Get the InternalIdentifier for the amenity
                DynamicParameters param = new DynamicParameters();
                param.Add("Id", id);
                param.Add("@EntityStatusId", (int)EntityStatusEnum.Active);

                string internalIdQuery = "SELECT InternalIdentifier FROM Amenities WHERE Id = @Id AND EntityStatusId = @EntityStatusId";
                string internalId = await _dataAccess.ReturnRowSql<string>(internalIdQuery, param);

                // Add premium decorators
                param = new DynamicParameters();
                param.Add("BaseAmenityId", id);
                param.Add("BaseAmenityInternalId", internalId);
                param.Add("@EntityStatusId", (int)EntityStatusEnum.Active);

                var premiumQuery = @"
                    SELECT Id, PremiumFeature, AdditionalCost, InternalIdentifier
                    FROM PremiumAmenityDecorators
                    WHERE (BaseAmenityId = @BaseAmenityId OR BaseAmenityInternalId = @BaseAmenityInternalId)
                        AND EntityStatusId = @EntityStatusId";

                var premiumDecorators = await _dataAccess.ReturnListSql<dynamic>(premiumQuery, param);

                foreach (var decorator in premiumDecorators)
                {
                    decoratedAmenity = new PremiumAmenityDecorator(
                        decoratedAmenity,
                        decorator.PremiumFeature,
                        decorator.AdditionalCost);
                }

                // Add seasonal decorators
                var seasonalQuery = @"
                    SELECT Id, Season, StartDate, EndDate, SeasonalPriceAdjustment, InternalIdentifier
                    FROM SeasonalAmenityDecorators
                    WHERE (BaseAmenityId = @BaseAmenityId OR BaseAmenityInternalId = @BaseAmenityInternalId)
                        AND EntityStatusId = @EntityStatusId";

                var seasonalDecorators = await _dataAccess.ReturnListSql<dynamic>(seasonalQuery, param);

                foreach (var decorator in seasonalDecorators)
                {
                    decoratedAmenity = new SeasonalAmenityDecorator(
                        decoratedAmenity,
                        decorator.Season,
                        decorator.StartDate,
                        decorator.EndDate,
                        decorator.SeasonalPriceAdjustment);
                }

                return decoratedAmenity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetDecoratedAmenityAsync)}");
                throw new RepositoryException(
                    "Failed to get decorated amenity",
                    REPOSITORY_NAME,
                    nameof(GetDecoratedAmenityAsync),
                    "GetDecorated",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<MiniBarAmenity> GetMiniBarAmenityByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetMiniBarAmenityByIdAsync)}");

                DynamicParameters param = new DynamicParameters();
                param.Add("Id", id);

                var query = $@"
                    SELECT a.*, at.AmenityType, mb.Id as MiniBarId, mb.IsComplimentary
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    INNER JOIN MiniBarAmenities mb ON a.Id = mb.AmenityId
                    WHERE a.Id = @Id AND at.AmenityType = {AmenityTypeLookup.minibar}
                        AND a.EntityStatusId = 1 AND mb.EntityStatusId = 1";

                var result = await _dataAccess.ReturnRowSql<dynamic>(query, param);
                if (result == null)
                    return null;

                // Get items for this minibar
                param = new DynamicParameters();
                param.Add("MiniBarAmenityId", result.MiniBarId);

                var itemsQuery = @"
                    SELECT Item 
                    FROM MiniBarItems 
                    WHERE MiniBarAmenityId = @MiniBarAmenityId AND EntityStatusId = 1";

                var items = await _dataAccess.ReturnListSql<string>(itemsQuery, param);

                return new MiniBarAmenity
                    (
                    result.Name,
                    result.Description,
                    result.PriceModifier,
                    result.IsStandard,
                    result.IsComplimentary,
                    items.ToList()                    
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetMiniBarAmenityByIdAsync)}");
                throw new RepositoryException(
                    "Failed to get MiniBar amenity by id",
                    REPOSITORY_NAME,
                    nameof(GetMiniBarAmenityByIdAsync),
                    "GetMiniBarById",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PremiumAmenityDecorator>> GetPremiumDecoratorsForAmenityAsync(int amenityId)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetPremiumDecoratorsForAmenityAsync)}");

                // Get base amenity first
                var baseAmenity = await GetAmenityByIdAsync(amenityId);
                if (baseAmenity == null)
                {
                    return Enumerable.Empty<PremiumAmenityDecorator>();
                }

                // Get premium decorators
                DynamicParameters param = new DynamicParameters();
                param.Add("BaseAmenityId", amenityId);

                string internalIdQuery = "SELECT InternalIdentifier FROM Amenities WHERE Id = @BaseAmenityId AND EntityStatusId = 1";
                string baseAmenityInternalId = await _dataAccess.ReturnRowSql<string>(internalIdQuery, param);

                if (!string.IsNullOrEmpty(baseAmenityInternalId))
                {
                    param.Add("BaseAmenityInternalId", baseAmenityInternalId);
                }

                var query = @"
                    SELECT Id, BaseAmenityId, PremiumFeature, AdditionalCost, InternalIdentifier, 
                           EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                    FROM PremiumAmenityDecorators
                    WHERE (BaseAmenityId = @BaseAmenityId 
                          OR (@BaseAmenityInternalId IS NOT NULL AND BaseAmenityInternalId = @BaseAmenityInternalId))
                          AND EntityStatusId = 1";

                var results = await _dataAccess.ReturnListSql<dynamic>(query, param);
                var decorators = new List<PremiumAmenityDecorator>();

                foreach (var result in results)
                {
                    var decorator = new PremiumAmenityDecorator(baseAmenity, result.PremiumFeature, result.AdditionalCost);

                    decorators.Add(decorator);
                }

                return decorators;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetPremiumDecoratorsForAmenityAsync)}");
                throw new RepositoryException(
                    "Failed to get premium decorators for amenity",
                    REPOSITORY_NAME,
                    nameof(GetPremiumDecoratorsForAmenityAsync),
                    "GetPremiumDecoratorsForAmenity",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<RoomServiceAmenity> GetRoomServiceAmenityByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetRoomServiceAmenityByIdAsync)}");

                DynamicParameters param = new DynamicParameters();
                param.Add("Id", id);

                var query = $@"
                    SELECT a.*, at.AmenityType, rs.HoursAvailable, rs.Is24Hours
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    INNER JOIN RoomServiceAmenities rs ON a.Id = rs.AmenityId
                    WHERE a.Id = @Id AND at.AmenityType = {AmenityTypeLookup.roomservice}
                        AND a.EntityStatusId = 1 AND rs.EntityStatusId = 1";

                var result = await _dataAccess.ReturnRowSql<dynamic>(query, param);
                if (result == null)
                    return null;

                return new RoomServiceAmenity(
                    result.Name,
                    result.Description,
                    result.PriceModifier, 
                    result.IsStandard, 
                    result.HoursAvailable, 
                    result.Is24Hours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetRoomServiceAmenityByIdAsync)}");
                throw new RepositoryException(
                    "Failed to get RoomService amenity by id",
                    REPOSITORY_NAME,
                    nameof(GetRoomServiceAmenityByIdAsync),
                    "GetRoomServiceById",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SeasonalAmenityDecorator>> GetSeasonalDecoratorsForAmenityAsync(int amenityId)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetSeasonalDecoratorsForAmenityAsync)}");

                // Get base amenity first
                var baseAmenity = await GetAmenityByIdAsync(amenityId);
                if (baseAmenity == null)
                {
                    return Enumerable.Empty<SeasonalAmenityDecorator>();
                }

                // Get seasonal decorators
                DynamicParameters param = new DynamicParameters();
                param.Add("BaseAmenityId", amenityId);

                string internalIdQuery = "SELECT InternalIdentifier FROM Amenities WHERE Id = @BaseAmenityId AND EntityStatusId = 1";
                string baseAmenityInternalId = await _dataAccess.ReturnRowSql<string>(internalIdQuery, param);

                if (!string.IsNullOrEmpty(baseAmenityInternalId))
                {
                    param.Add("BaseAmenityInternalId", baseAmenityInternalId);
                }

                var query = @"
                    SELECT Id, BaseAmenityId, Season, StartDate, EndDate, SeasonalPriceAdjustment, InternalIdentifier, 
                           EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                    FROM SeasonalAmenityDecorators
                    WHERE (BaseAmenityId = @BaseAmenityId 
                          OR (@BaseAmenityInternalId IS NOT NULL AND BaseAmenityInternalId = @BaseAmenityInternalId))
                          AND EntityStatusId = 1";

                var results = await _dataAccess.ReturnListSql<dynamic>(query, param);
                var decorators = new List<SeasonalAmenityDecorator>();

                foreach (var result in results)
                {
                    var decorator = new SeasonalAmenityDecorator(
                        baseAmenity,
                        result.Season,
                        result.StartDate,
                        result.EndDate,
                        result.SeasonalPriceAdjustment); 

                    decorators.Add(decorator);
                }

                return decorators;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetSeasonalDecoratorsForAmenityAsync)}");
                throw new RepositoryException(
                    "Failed to get seasonal decorators for amenity",
                    REPOSITORY_NAME,
                    nameof(GetSeasonalDecoratorsForAmenityAsync),
                    "GetSeasonalDecoratorsForAmenity",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BaseAmenity>> GetStandardAmenitiesAsync()
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetStandardAmenitiesAsync)}");

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@EntityStatusId", (int)EntityStatusEnum.Active);
                parameters.Add("@IsStandard", 1);

                var query = new StringBuilder();          

                query.Append(@"
                    SELECT a.*, at.AmenityType 
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    WHERE a.IsStandard = @IsStandard AND a.EntityStatusId = @EntityStatusId");

                var amenities = new List<BaseAmenity>();

                // First get all base amenities with their types
                var results = await _dataAccess.ReturnListSql<dynamic>(query.ToString(), parameters);

                foreach (var result in results)
                {
                    BaseAmenity amenity = await CreateAmenityFromDbResult(result);
                    if (amenity != null)
                    {
                        amenities.Add(amenity);
                    }
                }

                return amenities;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetStandardAmenitiesAsync)}");
                throw new RepositoryException(
                    "Failed to get standard amenities",
                    REPOSITORY_NAME,
                    nameof(GetStandardAmenitiesAsync),
                    "GetStandard",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task<WIFIAmenity> GetWifiAmenityByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(GetWifiAmenityByIdAsync)}");

                DynamicParameters param = new DynamicParameters();
                param.Add("Id", id);

                var query = $@"
                    SELECT a.*, at.AmenityType, w.NetworkName, w.Password, w.SpeedMbps
                    FROM Amenities a
                    INNER JOIN AmenityTypes at ON a.AmenityTypeId = at.Id
                    INNER JOIN WifiAmenities w ON a.Id = w.AmenityId
                    WHERE a.Id = @Id AND at.AmenityType = {AmenityTypeLookup.wifi} 
                        AND a.EntityStatusId = 1 AND w.EntityStatusId = 1";

                var result = await _dataAccess.ReturnRowSql<dynamic>(query, param);
                if (result == null)
                    return null;

                return new WIFIAmenity(
                    result.Name, 
                    result.Description, 
                    result.PriceModifier, 
                    result.IsStandard, 
                    result.NetworkName, 
                    result.Password, 
                    result.SpeedMbps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(GetWifiAmenityByIdAsync)}");
                throw new RepositoryException(
                    "Failed to get WiFi amenity by id",
                    REPOSITORY_NAME,
                    nameof(GetWifiAmenityByIdAsync),
                    "GetWifiById",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveDecoratorAsync(int decoratorId)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(RemoveDecoratorAsync)}");

                await _dataAccess.ExecuteInTransaction(async (connection, transaction, token) =>
                {
                    // Try to delete from both decorator tables (only one will succeed)
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Id", decoratorId);
                    param.Add("UpdatedAt", DateTime.UtcNow);
                    param.Add("UpdatedBy", 0); // Replace with actual user ID when available
                    param.Add("EntityStatusId", (int)EntityStatusEnum.DeletedForEveryone); // DeletedForEveryone

                    // Update premium decorators
                    string premiumSql = @"
                        UPDATE PremiumAmenityDecorators 
                        SET EntityStatusId = @EntityStatusId,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE Id = @Id";

                    await _dataAccess.ExecuteWithoutReturnSqlInTransaction(premiumSql, transaction, param, token);

                    // Update seasonal decorators
                    string seasonalSql = @"
                        UPDATE SeasonalAmenityDecorators 
                        SET EntityStatusId = @EntityStatusId,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE Id = @Id";

                    await _dataAccess.ExecuteWithoutReturnSqlInTransaction(seasonalSql, transaction, param, token);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(RemoveDecoratorAsync)}");
                throw new RepositoryException(
                    "Failed to remove decorator",
                    REPOSITORY_NAME,
                    nameof(RemoveDecoratorAsync),
                    "RemoveDecorator",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAmenityAsync(BaseAmenity amenity)
        {
            try
            {
                _logger.LogInformation($"Processing {REPOSITORY_NAME}.{nameof(UpdateAmenityAsync)}");

                await _dataAccess.ExecuteInTransaction(async (connection, transaction, token) =>
                {
                    // Update the base amenity
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Id", amenity.Id);
                    param.Add("Name", amenity.Name);
                    param.Add("Description", amenity.Description);
                    param.Add("PriceModifier", amenity.PriceModifier);
                    param.Add("IsStandard", amenity.IsStandard);
                    param.Add("UpdatedAt", DateTime.UtcNow);
                    param.Add("UpdatedBy", amenity.UpdatedBy);

                    StringBuilder sql = new StringBuilder();
                    sql.Append(@"
                        UPDATE Amenities 
                        SET Name = @Name,
                            Description = @Description,
                            PriceModifier = @PriceModifier,
                            IsStandard = @IsStandard,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE Id = @Id AND EntityStatusId = 1");

                    await _dataAccess.ExecuteWithoutReturnSqlInTransaction(
                        sql.ToString(), transaction, param, token);

                    // Now update type-specific details
                    if (amenity is WIFIAmenity wifiAmenity)
                    {
                        await UpdateWifiAmenityDetails(transaction, amenity.Id, wifiAmenity, token);
                    }
                    else if (amenity is MiniBarAmenity miniBarAmenity)
                    {
                        await UpdateMiniBarAmenityDetails(transaction, amenity.Id, miniBarAmenity, token);
                    }
                    else if (amenity is RoomServiceAmenity roomServiceAmenity)
                    {
                        await UpdateRoomServiceAmenityDetails(transaction, amenity.Id, roomServiceAmenity, token);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {REPOSITORY_NAME}.{nameof(UpdateAmenityAsync)}");
                throw new RepositoryException(
                    "Failed to update amenity",
                    REPOSITORY_NAME,
                    nameof(UpdateAmenityAsync),
                    "Update",
                    ex);
            }
        }

        #region Helper Methods
        private async Task<BaseAmenity> CreateAmenityFromDbResult(dynamic result)
        {
            try
            {
                string amenityType = result.AmenityType?.ToLowerInvariant();

                BaseAmenity amenity = null;

                switch (amenityType)
                {
                    case AmenityTypeLookup.wifi:
                        amenity = await GetWifiAmenityByIdAsync(result.Id);
                        break;

                    case AmenityTypeLookup.minibar:
                        amenity = await GetMiniBarAmenityByIdAsync(result.Id);
                        break;

                    case AmenityTypeLookup.roomservice:
                        amenity = await GetRoomServiceAmenityByIdAsync(result.Id);
                        break;
                    default:
                        // Create a base amenity as fallback
                        throw new ArgumentException($"Unknown amenity type: {amenityType}");
                }

                return amenity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating amenity from database result");
                throw new RepositoryException(
                    "Failed to create amenity from database result",
                    REPOSITORY_NAME,
                    "CreateAmenityFromDbResult",
                    "Helper",
                    ex);
            }
        }

        private string GetAmenityTypeName(BaseAmenity amenity)
        {
            if (amenity is WIFIAmenity)
            {
                return "Wifi";
            }
            else if (amenity is MiniBarAmenity)
            {
                return "Minibar";
            }
            else if (amenity is RoomServiceAmenity)
            {
                return "RoomService";
            }
            else
            {
                return "basic";
            }
        }

        private async Task<int> GetOrCreateAmenityTypeId(IDbTransaction transaction, string amenityTypeName)
        {
            try
            {
                // Try to get existing type ID
                DynamicParameters param = new DynamicParameters();
                param.Add("AmenityType", amenityTypeName);

                string query = "SELECT Id FROM AmenityTypes WHERE AmenityType = @AmenityType AND EntityStatusId = 1";

                int? typeId = await _dataAccess.ReturnRowSqlInTransaction<int?>(query, transaction, param);

                if (typeId.HasValue)
                {
                    return typeId.Value;
                }

                // Create new type
                DynamicParameters createParam = new DynamicParameters();
                createParam.Add("AmenityType", amenityTypeName);
                createParam.Add("EntityStatusId", 1); // Active
                createParam.Add("CreatedAt", DateTime.UtcNow);
                createParam.Add("CreatedBy", 0); // Replace with actual user ID when available
                createParam.Add("UpdatedAt", DateTime.UtcNow);
                createParam.Add("UpdatedBy", 0); // Replace with actual user ID when available

                string createQuery = @"
                    INSERT INTO AmenityTypes 
                        (AmenityType, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                    VALUES 
                        (@AmenityType, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                return await _dataAccess.ReturnRowSqlInTransaction<int>(createQuery, transaction, createParam);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting or creating amenity type: {amenityTypeName}");
                throw new RepositoryException(
                    $"Failed to get or create amenity type: {amenityTypeName}",
                    REPOSITORY_NAME,
                    "GetOrCreateAmenityTypeId",
                    "Helper",
                    ex);
            }
        }

        private async Task AddWifiAmenityDetails(IDbTransaction transaction, int amenityId, WIFIAmenity wifiAmenity, System.Threading.CancellationToken? token = null)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("AmenityId", amenityId);
            param.Add("NetworkName", wifiAmenity.NetworkName);
            param.Add("Password", wifiAmenity.Password);
            param.Add("SpeedMbps", wifiAmenity.SpeedMbps);
            param.Add("EntityStatusId", 1); // Active
            param.Add("CreatedAt", DateTime.UtcNow);
            param.Add("CreatedBy", wifiAmenity.CreatedBy);
            param.Add("UpdatedAt", DateTime.UtcNow);
            param.Add("UpdatedBy", wifiAmenity.UpdatedBy);

            string sql = @"
                INSERT INTO WifiAmenities 
                    (AmenityId, NetworkName, Password, SpeedMbps, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                VALUES 
                    (@AmenityId, @NetworkName, @Password, @SpeedMbps, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            await _dataAccess.ExecuteWithoutReturnSqlInTransaction(sql, transaction, param, cancellationToken: token.Value);
        }

        private async Task AddMiniBarAmenityDetails(IDbTransaction transaction, int amenityId, MiniBarAmenity miniBarAmenity, System.Threading.CancellationToken? token = null)
        {
            // Add MiniBar details
            DynamicParameters param = new DynamicParameters();
            param.Add("AmenityId", amenityId);
            param.Add("IsComplimentary", miniBarAmenity.IsComplimentary);
            param.Add("EntityStatusId", 1); // Active
            param.Add("CreatedAt", DateTime.UtcNow);
            param.Add("CreatedBy", miniBarAmenity.CreatedBy);
            param.Add("UpdatedAt", DateTime.UtcNow);
            param.Add("UpdatedBy", miniBarAmenity.UpdatedBy);

            string sql = @"
                INSERT INTO MiniBarAmenities 
                    (AmenityId, IsComplimentary, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                VALUES 
                    (@AmenityId, @IsComplimentary, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            int miniBarId = await _dataAccess.ReturnRowSqlInTransaction<int>(sql, transaction, param, cancellationToken: token.Value);

            // Add items if any
            if (miniBarAmenity.Items != null && miniBarAmenity.Items.Any())
            {
                foreach (var item in miniBarAmenity.Items)
                {
                    DynamicParameters itemParam = new DynamicParameters();
                    itemParam.Add("MiniBarAmenityId", miniBarId);
                    itemParam.Add("Item", item);
                    itemParam.Add("EntityStatusId", 1); // Active
                    itemParam.Add("CreatedAt", DateTime.UtcNow);
                    itemParam.Add("CreatedBy", miniBarAmenity.CreatedBy);
                    itemParam.Add("UpdatedAt", DateTime.UtcNow);
                    itemParam.Add("UpdatedBy", miniBarAmenity.UpdatedBy);

                    string itemSql = @"
                        INSERT INTO MiniBarItems 
                            (MiniBarAmenityId, Item, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                        VALUES 
                            (@MiniBarAmenityId, @Item, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

                    await _dataAccess.ExecuteWithoutReturnSqlInTransaction(itemSql, transaction, itemParam, cancellationToken: token.Value);
                }
            }
        }

        private async Task AddRoomServiceAmenityDetails(IDbTransaction transaction, int amenityId, RoomServiceAmenity roomServiceAmenity, System.Threading.CancellationToken? token = null)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("AmenityId", amenityId);
            param.Add("HoursAvailable", roomServiceAmenity.HoursAvailable);
            param.Add("Is24Hours", roomServiceAmenity.Is24Hours);
            param.Add("EntityStatusId", 1); // Active
            param.Add("CreatedAt", DateTime.UtcNow);
            param.Add("CreatedBy", roomServiceAmenity.CreatedBy);
            param.Add("UpdatedAt", DateTime.UtcNow);
            param.Add("UpdatedBy", roomServiceAmenity.UpdatedBy);

            string sql = @"
                INSERT INTO RoomServiceAmenities 
                    (AmenityId, HoursAvailable, Is24Hours, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                VALUES 
                    (@AmenityId, @HoursAvailable, @Is24Hours, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            await _dataAccess.ExecuteWithoutReturnSqlInTransaction(sql, transaction, param, cancellationToken: token.Value);
        }

        private async Task UpdateWifiAmenityDetails(IDbTransaction transaction, int amenityId, WIFIAmenity wifiAmenity, System.Threading.CancellationToken? token = null)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("AmenityId", amenityId);
            param.Add("NetworkName", wifiAmenity.NetworkName);
            param.Add("Password", wifiAmenity.Password);
            param.Add("SpeedMbps", wifiAmenity.SpeedMbps);
            param.Add("UpdatedAt", DateTime.UtcNow);
            param.Add("UpdatedBy", wifiAmenity.UpdatedBy);

            string sql = @"
                UPDATE WifiAmenities 
                SET NetworkName = @NetworkName,
                    Password = @Password,
                    SpeedMbps = @SpeedMbps,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE AmenityId = @AmenityId AND EntityStatusId = 1";

            await _dataAccess.ExecuteWithoutReturnSqlInTransaction(sql, transaction, param, cancellationToken: token.Value);
        }

        private async Task UpdateMiniBarAmenityDetails(IDbTransaction transaction, int amenityId, MiniBarAmenity miniBarAmenity, System.Threading.CancellationToken? token = null)
        {
            // Update MiniBar details
            DynamicParameters param = new DynamicParameters();
            param.Add("AmenityId", amenityId);
            param.Add("IsComplimentary", miniBarAmenity.IsComplimentary);
            param.Add("UpdatedAt", DateTime.UtcNow);
            param.Add("UpdatedBy", miniBarAmenity.UpdatedBy);

            string sql = @"
                UPDATE MiniBarAmenities 
                SET IsComplimentary = @IsComplimentary,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE AmenityId = @AmenityId AND EntityStatusId = 1";

            await _dataAccess.ExecuteWithoutReturnSqlInTransaction(sql, transaction, param, cancellationToken: token.Value);

            // Get the MiniBarAmenityId
            string idQuery = "SELECT Id FROM MiniBarAmenities WHERE AmenityId = @AmenityId AND EntityStatusId = 1";
            int miniBarId = await _dataAccess.ReturnRowSqlInTransaction<int>(idQuery, transaction, param, cancellationToken: token.Value);

            // Delete existing items
            DynamicParameters deleteParam = new DynamicParameters();
            deleteParam.Add("MiniBarAmenityId", miniBarId);

            string deleteSql = @"
                UPDATE MiniBarItems 
                SET EntityStatusId = 3,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE MiniBarAmenityId = @MiniBarAmenityId";

            await _dataAccess.ExecuteWithoutReturnSqlInTransaction(deleteSql, transaction, deleteParam, cancellationToken: token.Value);

            // Add new items
            if (miniBarAmenity.Items != null && miniBarAmenity.Items.Any())
            {
                foreach (var item in miniBarAmenity.Items)
                {
                    DynamicParameters itemParam = new DynamicParameters();
                    itemParam.Add("MiniBarAmenityId", miniBarId);
                    itemParam.Add("Item", item);
                    itemParam.Add("EntityStatusId", 1); // Active
                    itemParam.Add("CreatedAt", DateTime.UtcNow);
                    itemParam.Add("CreatedBy", miniBarAmenity.CreatedBy);
                    itemParam.Add("UpdatedAt", DateTime.UtcNow);
                    itemParam.Add("UpdatedBy", miniBarAmenity.UpdatedBy);

                    string itemSql = @"
                        INSERT INTO MiniBarItems 
                            (MiniBarAmenityId, Item, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                        VALUES 
                            (@MiniBarAmenityId, @Item, @EntityStatusId, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

                    await _dataAccess.ExecuteWithoutReturnSqlInTransaction(itemSql, transaction, itemParam, cancellationToken: token.Value);
                }
            }
        }

        private async Task UpdateRoomServiceAmenityDetails(IDbTransaction transaction, int amenityId, RoomServiceAmenity roomServiceAmenity, System.Threading.CancellationToken? token = null)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("AmenityId", amenityId);
            param.Add("HoursAvailable", roomServiceAmenity.HoursAvailable);
            param.Add("Is24Hours", roomServiceAmenity.Is24Hours);
            param.Add("UpdatedAt", DateTime.UtcNow);
            param.Add("UpdatedBy", roomServiceAmenity.UpdatedBy);

            string sql = @"
                UPDATE RoomServiceAmenities 
                SET HoursAvailable = @HoursAvailable,
                    Is24Hours = @Is24Hours,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE AmenityId = @AmenityId AND EntityStatusId = 1";

            await _dataAccess.ExecuteWithoutReturnSqlInTransaction(sql, transaction, param, cancellationToken: token.Value);
        }
        #endregion Helper Methods
    }
}
