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
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

try
{
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        options.Filters.Add(new ProducesAttribute("application/json", "text/json", "application/xml", "text/xml"));
        options.ReturnHttpNotAcceptable = true;
    }).AddNewtonsoftJson(setupAction =>
    {
        setupAction.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
    })
   .AddXmlDataContractSerializerFormatters();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    #region CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
        {
            builder.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
        });
    });
    #endregion

    #region compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });
    #endregion

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

    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    app.UseMiddleware<CustomExceptionsHandlerMiddleware>();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.UseRouting();

    app.UseCors(MyAllowSpecificOrigins);

    app.UseAuthentication();

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
