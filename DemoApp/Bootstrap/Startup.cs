using System.Data;
using DemoApp.Bootstrap.Jwt;
using DemoApp.Domain;
using DemoApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace DemoApp.Bootstrap
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureJwtAuthentication(options =>
            {
                options.Audience = Configuration["Jwt:Audience"];
                options.Authority = Configuration["Jwt:Issuer"];
                options.TokenValidationParameters.ValidIssuer = options.Authority;
            });

            services.ConfigureJwtAuthorization();

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
            });

            services.AddScoped<IArticlesRepository, DapperArticlesRepository>();
            services.AddScoped<IDbConnection>((sp) =>
                new NpgsqlConnection(Configuration.GetValue<string>("DB:ConnectionString")));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}