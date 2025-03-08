using Autofac.Extensions.DependencyInjection;
using Autofac;
using Dapper.API.Configure;
using Dapper.API.Data.Dapper;
using Dapper.API.Logging;
using Dapper.API.Middlewares;
using Dapper.API.Models.AppSettings;
using Serilog;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    #region JWT  
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretForKey"]))
            };
        });
    #endregion

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

    builder.Services.AddSingleton<ICreateToken,CreateToken>();
    #endregion

    #region AutoFac
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacModuleRegister()));
    #endregion

    #region Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.ConfigureSwagger(builder.Configuration);
    #endregion Swagger

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
