using UniversityCourses.Application.Dtos;
using UniversityCourses.Application.Services;
using UniversityCourses.Domain.Interfaces;
using UniversityCourses.Infrastructure.Data;
using UniversityCourses.Infrastructure.Repositories;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ValfriDbConnectionString")));


//Services
builder.Services.AddScoped<CourseService>();

//Repos
builder.Services.AddScoped<ICourseRepository, CourseRepository>();


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




#region Courses
var courses = app.MapGroup("/api/courses");

courses.MapPost("/", async (CreateCourseDto dto, CourseService courseService) =>
{
    var result = await courseService.CreateCourseAsync(dto);

    return result.IsSucess
        ? Results.Created("", result.Data)
        : Results.Conflict(result.Message);
});

courses.MapGet("/", async (CourseService courseService) =>
{
    var result = await courseService.GetAllCoursesAsync();
    return Results.Ok(result.Data);
});
#endregion




app.Run();


