using Api;
using Api.Middleware;
using AutoMapper;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Data.Repository;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conDefault = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conDefault));


builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddTransient<IRepository, Repository>();

ProgramSetup.AddDepedencyInjectionService(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<MiddlewareHandling>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
