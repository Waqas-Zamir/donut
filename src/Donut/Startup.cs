// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut
{
    using Donut.Services;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization(
                    options =>
                    {
                        options.AddPolicy("admin_policy", policy => policy.RequireRole("admin"));
                    });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(
                    options =>
                    {
                        options.Authority = this.configuration.GetValue<string>("Api-Authority");
                        options.ApiName = this.configuration.GetValue<string>("Api-Name");
                        options.ApiSecret = this.configuration.GetValue<string>("Api-Secret");

                        // NOTE (Cameron): This is only used because we're performing HTTPS termination at the proxy.
                        options.RequireHttpsMetadata = false;
                    });

            services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();
            services.AddSingleton<ProxyService>();
            services.Configure<DonutSettings>(this.configuration);

            services.AddCors(options =>
            {
                ////options.AddPolicy("spa", policy =>
                ////{
                ////    policy.WithOrigins("http://localhost:5008")
                ////        .AllowAnyHeader()
                ////        .AllowAnyMethod();
                ////});
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }

            ////app.UseCors("spa");

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
