
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Dapper.API.Configure
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Dapper API",
                    Version = "v1",
                    Description = "API to display how to builder a dapper API",
                    Contact = new OpenApiContact
                    {
                        Name = "Trendon Cato",
                        Email = "trendoncato@google.com",
                        //Url = new Uri("https://your-website.com")
                    },
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Your License",
                    //    Url = new Uri("https://your-license-url.com")
                    //}
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

            }); 
        }
    }
}
