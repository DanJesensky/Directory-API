using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Directory.Abstractions;
using Directory.Data;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Directory.Api {
    [ExcludeFromCodeCoverage]
    public class Startup {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddLogging();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => _configuration.Bind("JwtBearerOptions", opt));

            services.AddAuthorization(authzBuilder => {
                authzBuilder
                    .AddPolicy(Constants.AuthorizationPolicies.DefaultPolicy,
                               policyBuilder => 
                                   policyBuilder.RequireClaim(JwtClaimTypes.Subject));

                authzBuilder.DefaultPolicy = authzBuilder.GetPolicy(Constants.AuthorizationPolicies.DefaultPolicy);
            });

            // JwtSecurityTokenHandler by default maps some OAuth 2.0 claims to long WS-style claims.
            // Clearing this map prevents that. https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/415
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthorization();

            services.AddControllers();

            services.AddDbContext<DirectoryContext>(ctxBuilder =>
                ctxBuilder.UseMySQL(_configuration["DatabaseConnectionString"]));

            services.AddScoped<IServiceHealthProvider, ServiceHealthProvider>()
                    .AddScoped<ClaimsPrincipal>(provider => provider
                                                            .GetRequiredService<IHttpContextAccessor>()
                                                            .HttpContext
                                                            .User);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IDefaultPictureProvider, DefaultPictureProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
