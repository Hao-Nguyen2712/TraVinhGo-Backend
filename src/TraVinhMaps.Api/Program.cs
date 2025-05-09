// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application;
using TraVinhMaps.Application.External.Models;
using TraVinhMaps.Infrastructure;
using TraVinhMaps.Infrastructure.Db.Data;

var root = Directory.GetCurrentDirectory();
var dotnetEnv = Path.Combine(root, ".env");

if (File.Exists(dotnetEnv))
{
    DotNetEnv.Env.Load(dotnetEnv);
}
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .AddEnvironmentVariables();
builder.Services.Configure<MongoDbSetting>(options =>
{
    builder.Configuration.GetSection("MongoDb").Bind(options);

    var envConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    var envDatabaseName = Environment.GetEnvironmentVariable("DbName");

    if (!string.IsNullOrEmpty(envConnectionString))
    {
        options.ConnectionString = envConnectionString;
    }

    if (!string.IsNullOrEmpty(envDatabaseName))
    {
        options.DatabaseName = envDatabaseName;
    }

    Console.WriteLine(options.DatabaseName + "/n" + options.ConnectionString);
});
// layer di
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//config Cloudinary
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (builder.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceScope = scope.ServiceProvider;
        await DataSeeding.SeedData(serviceScope);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
