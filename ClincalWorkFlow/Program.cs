using ClincalWorkFlow.Services;
using Microsoft.AspNetCore.Http.Features;      // if you later tweak upload limits
using Microsoft.AspNetCore.HttpOverrides;     // forwarded headers (Render proxy)

namespace ClincalWorkFlow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // === Services ===
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ML.NET prediction service
            builder.Services.AddSingleton<PredictionEngineService>();

            // CORS (must be BEFORE Build)
            var allowedOrigins = new[]
            {
                "https://xray-frontend-6piq.onrender.com", // Render frontend
                "http://localhost:5173",                   // Vite local
                "http://localhost:3000"                    // CRA local
            };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            var app = builder.Build();

            // === Middleware ===

            // (Optional) Swagger in prod while you debug; remove later if you want.
            app.UseSwagger();
            app.UseSwaggerUI();

            // Forwarded headers (good hygiene behind Render's proxy)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthorization();

            app.MapControllers();

            // quick health check endpoint
            app.MapGet("/health", () => Results.Ok("OK"));

            app.Run();
        }
    }
}
