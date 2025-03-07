using Dapper.API.Data.Dapper;
using Dapper.API.Models.AppSettings;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region AppSettings Configuration
var connectionStrings = new ConnectionStrings();
builder.Configuration.Bind(key: nameof(ConnectionStrings), connectionStrings);
builder.Services.AddSingleton(connectionStrings);

#endregion AppSettings Configuration

#region Data Access Dependency Injection
builder.Services.AddSingleton<IDapperHandler, DapperHandler>();
#endregion


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
