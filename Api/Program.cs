using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
using Shared.Interfaces;
using Infrastructure.Data.Repository;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddAutoMapper(typeof(MapperProfileExt));

var conDefault = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conDefault));

builder.Services.AddTransient<IRepository, Repository>();

#region[DI] Add Depedency Injection here
#endregion

#region[DI2]Auto Create Depedency Injection

#endregion

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
