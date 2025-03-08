using Dapper.API.Data.Dapper;
using Dapper.API.Logging;
using Dapper.API.Middlewares;
using Dapper.API.Models.AppSettings;
using Dapper.API.Services;
using Dapper.API.Services.Interfaces;
using Serilog;
using StackExchange.Redis;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    #region Serilog
    builder.Host.UseSerilog(SeriLogger.Configure);
    #endregion Serilog

    #region Redis
    var RedisHost = builder.Configuration["ConnectionStrings:RedisHost"];
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(RedisHost));
    #endregion

    #region AppSettings Configuration
    var connectionStrings = new ConnectionStrings();
    builder.Configuration.Bind(key: nameof(ConnectionStrings), connectionStrings);
    builder.Services.AddSingleton(connectionStrings);
    #endregion AppSettings Configuration

    #region Data Access Dependency Injection
    builder.Services.AddSingleton<IDapperHandler, DapperHandler>();

    builder.Services.AddTransient<CustomExceptionsHandlerMiddleware>();

    builder.Services.AddScoped<IRedisService,RedisService>();
    #endregion


    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseMiddleware<CustomExceptionsHandlerMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();


    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
