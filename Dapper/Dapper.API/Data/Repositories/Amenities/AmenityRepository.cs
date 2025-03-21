using Dapper.API.Data.Dapper;
using Dapper.API.Data.Repositories.Interfaces.Amenities;
using Dapper.API.Entities.Amentities;
using Dapper.API.Enums.StandardEnums;
using Dapper.API.Exceptions;
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<int> AddSeasonalDecoratorAsync(SeasonalAmenityDecorator decorator)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task DeleteAmenityAsync(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<RoomServiceAmenity>> GetAllRoomServiceAmenitiesAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WIFIAmenity>> GetAllWifiAmenitiesAsync()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PremiumAmenityDecorator>> GetPremiumDecoratorsForAmenityAsync(int amenityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<RoomServiceAmenity> GetRoomServiceAmenityByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SeasonalAmenityDecorator>> GetSeasonalDecoratorsForAmenityAsync(int amenityId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task RemoveDecoratorAsync(int decoratorId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task UpdateAmenityAsync(BaseAmenity amenity)
        {
            throw new NotImplementedException();
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
                    case "wifi":
                        amenity = await GetWifiAmenityByIdAsync(result.Id);
                        break;

                    case "minibar":
                        amenity = await GetMiniBarAmenityByIdAsync(result.Id);
                        break;

                    case "roomservice":
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
        #endregion Helper Methods
    }
}
