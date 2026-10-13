using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Models;
using CollegeManagementApi.Data;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Services;
using Scalar.AspNetCore;  
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(Options =>
{
    Options.UseSqlite("DataSource = students1.db");
});
builder.Services.AddScoped<ICollegeService, CollegeService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") // your frontend's origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
