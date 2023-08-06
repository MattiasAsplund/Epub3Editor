using System.Web.Http;
using Epub3Editor.Database;
using Epub3Editor.Shared.Interfaces;
using Epub3Editor.xSystem;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IEpub3Database, NpgsqlDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=Epub3System;User ID=postgres;Password=postgres");
});
builder.Services.AddScoped<Epub3System>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
    
var app = builder.Build();

var scope = app.Services.CreateScope();
var database = scope.ServiceProvider.GetService<IEpub3Database>();
((DbContext)database).Database.Migrate();

app.UseCors();

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