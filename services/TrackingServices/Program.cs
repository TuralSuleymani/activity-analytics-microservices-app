using Microsoft.AspNetCore.Authentication.JwtBearer;
using TrackingService.Extensions;
using Services = TrackingService.Services;
using TrackingServices.Repositories;

namespace TrackingServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //jwt
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.Authority = builder.Configuration["JwtSettings:Authority"];
                     options.Audience = builder.Configuration["JwtSettings:Audience"];
                 });

            builder.Services.AddCosmosDbClient(builder.Configuration);
            builder.Services.AddServiceBusClient(builder.Configuration);

            builder.Services.AddTransient<Services.ITrackingService, Services.TrackingService>();

            builder.Services.AddSingleton<IUserActivityRepository, CosmosDbUserActivityRepository>();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
