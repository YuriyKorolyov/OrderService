using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.Behaviors;
using OrderService.WebApi.Clients;
using OrderService.WebApi.Events;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.Validators;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HTTP client and Refit wrapper for PaymentService
builder.Services.AddHttpClient("PaymentService", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["PaymentService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException(
            "PaymentService base URL is not configured. " +
            "Please set 'PaymentService:BaseUrl' in configuration.");
    }

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IPaymentServiceClient>(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("PaymentService");
    return RestService.For<IPaymentServiceClient>(httpClient);
});

// Add mappers
builder.Services.AddSingleton<OrderMapper>();

// Add Kafka producer
builder.Services.AddSingleton<IOrderEventProducer, KafkaOrderEventProducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
