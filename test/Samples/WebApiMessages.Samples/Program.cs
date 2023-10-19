using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApiMessages.Samples.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ctx =>
    {
        ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return new JsonResult(new BadRequestMessage(ctx.ModelState).ProblemDetails);
    };
});

#pragma warning disable ASP0000
builder.Services.AddDbContext<WeatherForecastDbContext>(opt => opt.UseInMemoryDatabase("WeatherForecast"));
using (var services = builder.Services.BuildServiceProvider())
{
    using (var context = services.GetRequiredService<WeatherForecastDbContext>())
    {
        context.Database.EnsureCreated();

        var summaries = new[]
         {
             "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        await context.AddRangeAsync(Enumerable.Range(0, summaries.Length).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }));
        await context.SaveChangesAsync();
    }
}
#pragma warning restore ASP0000

// builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
app.UseApiMessageHandler();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }