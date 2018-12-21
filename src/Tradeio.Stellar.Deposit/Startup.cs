using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Deposit.Controllers;
using Tradeio.Stellar.Interfaces;

namespace Tradeio.Stellar.Deposit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwagger();

            services.AddTransient<IStellarConfigurationService, StellarConfigurationService>();
            services.AddTransient<IDepositAddressService, DepositAddressService>();
            services.AddTransient<IAddressController, DepositsControllerInternal>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.SwaggerUiRoute = "/swagger";
                settings.GeneratorSettings.Version = "1.0.0";
                settings.GeneratorSettings.Title = "Stellar Deposit API";
                settings.GeneratorSettings.Description = "";
            });

            app.UseSwaggerReDocWithApiExplorer(settings => settings.SwaggerUiRoute = "/docs");
        }
    }
}