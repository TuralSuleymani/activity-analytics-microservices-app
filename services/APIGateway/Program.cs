using Azure.Monitor.OpenTelemetry.AspNetCore;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry.Resources;

namespace APIGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ocelot config
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
            {
                //Gateway traffic can be high, minimize load with sampling
                options.SamplingRatio = 0.2f; // 20%
            });

            builder.Services.AddOpenTelemetry().ConfigureResource(r =>
                r.AddService(serviceName: "api-gateway"));

            builder.Services.AddOcelot();

            // Auth
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://your-auth-provider.com";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = "api_gateway",
                        ValidIssuer = "https://your-auth-provider.com"
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.UseOcelot();

            app.Run();
        }
    }
}
