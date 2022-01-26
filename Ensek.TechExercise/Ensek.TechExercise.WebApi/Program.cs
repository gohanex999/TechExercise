using Ensek.TechExercise.DataAccess.Sql;
using Ensek.TechExercise.Domain.Services;
using Ensek.TechExercise.WebApi.Context;
using Ensek.TechExercise.WebApi.Repositories;
using Ensek.TechExercise.WebApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IEnergyConsumptionMonitorService, EnergyConsumptionMonitorService>();
builder.Services.AddTransient<IMeterReadingParser, MeterReadingParser>();
builder.Services.AddTransient<IMeterReaderManager, MeterReadingManager>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IMeterReadingRepository, MeterReadingRepository>();

builder.Services.AddDbContext<EnsekDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EnsekDb")));
var serviceProvider = builder.Services.BuildServiceProvider();
var context = serviceProvider.GetRequiredService<EnsekDbContext>();
DataSeeder.Seed(context);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
