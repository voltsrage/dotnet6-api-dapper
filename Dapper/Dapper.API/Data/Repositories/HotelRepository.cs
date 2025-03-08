﻿using Dapper.API.Data.Dapper;
using Dapper.API.Data.Repositories.Interfaces;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Entities;
using Dapper.API.Enums.StandardEnums;
using Dapper.API.Exceptions;
using Dapper.API.Helpers;
using Dapper.API.Models.Pagination;
using System.Text;

namespace Dapper.API.Data.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly IDapperHandler _dataAccess;
        private readonly ILogger<HotelRepository> _logger;
        private readonly PaginationHelper _paginationHelper;

        public HotelRepository(ILogger<HotelRepository> logger, IDapperHandler dataAccess, PaginationHelper paginationHelper)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _paginationHelper = paginationHelper;
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
        /// <param name="pagination"></param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>Paginated result containing hotels and metadata</returns>
        public async Task<PaginatedResult<Hotel>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default)
        {
            try
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
                    "Hotels",
                    columns,
                    searchableColumns,
                    baseCondition,
                    sortColumn:"Id",
                    cancellationToken:cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated hotels - Page: {Page}, PageSize: {PageSize}", pagination.Page, pagination.PageSize);

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

                sql.Append(@"SELECT Id, Name, Address, City, Country, PhoneNumber, Email, CreatedAt, EntityStatusId FROM Hotels WHERE Id = @Id");

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

                sql.Append(@"SELECT Id, Name, Address, City, Country, PhoneNumber, Email, CreatedAt, EntityStatusId FROM Hotels WHERE Name = @Name");

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
                    nameof(HotelRepository),
                    nameof(GetHotelByNameAndAddress),
                    "GetByNameAndAddress",
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
                    nameof(HotelRepository),
                    nameof(UpdateHotel),
                    "Update",
                    ex,
                    null);
            }
        }
    }
}
