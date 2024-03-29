﻿using System;
using Directory.Abstractions;
using Directory.Data;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

            services.AddAuthorization(authorizationBuilder => {
                authorizationBuilder
                    .AddPolicy(Constants.AuthorizationPolicies.DefaultPolicy,
                        policyBuilder => 
                            policyBuilder
                                .RequireAuthenticatedUser()
                                .RequireClaim(JwtClaimTypes.Subject));

                authorizationBuilder.DefaultPolicy = authorizationBuilder.GetPolicy(Constants.AuthorizationPolicies.DefaultPolicy);
            });

            // JwtSecurityTokenHandler by default maps some OAuth 2.0 claims to long WS-style claims.
            // Clearing this map prevents that. https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/415
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddControllers();

            services.AddDbContext<DirectoryContext>(ctxBuilder =>
                ctxBuilder.UseMySQL(_configuration[Constants.Config.DatabaseConnectionString]));

            #region Local Factories

            static ClaimsPrincipal GetPrincipalFromContext(IServiceProvider provider) =>
                provider.GetRequiredService<IHttpContextAccessor>().HttpContext.User;

            #endregion Local Factories

            services
                .AddScoped<IServiceHealthProvider, ServiceHealthProvider>()
                .AddScoped<ClaimsPrincipal>(GetPrincipalFromContext);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IDefaultPictureProvider, DefaultPictureProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
