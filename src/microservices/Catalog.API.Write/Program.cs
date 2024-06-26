﻿using Catalog.API.Write.EventsHandling;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using JwtLibrary;
using Catalog.Infrastructure.Services;
using Extensions;
using Catalog.API.Write.Extensions;

namespace Catalog.API.Write;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddContextExtension<CatalogContext>(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddSwaggerServiceExtension();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisCache:ConnectionString"];
        });

        builder.Services.AddTransient<ICacheService, RedisCacheService>();
        builder.Services.AddTransient<ICatalogRepository, CatalogRepository>();

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddEventExtension(builder.Configuration);

        builder.Services.AddHealthCheckExtensions(builder.Configuration);

        builder.Services.AddCors(options =>
            options.AddPolicy("newPolicy", policy => policy/*.WithOrigins("localhost")*/.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseSwagger(c => { c.RouteTemplate = "/swagger/{documentName}/swagger.json"; });
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseCors("newPolicy");

        app.Run();
    }
}

