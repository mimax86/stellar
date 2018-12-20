using Tradeio.Stellar.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Deposit.Controllers;

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

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IBalanceService, BalanceService>();

            services.AddTransient<IStellarConfigurationService, StellarConfigurationService>();
            services.AddTransient<IStellarRepository, StellarRepository>();

            services.AddTransient<IDepositsController, DepositsControllerInternal>();
            services.AddTransient<IStatusController, StatusControllerInternal>();

            services.AddSingleton<DepositProcessor>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
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

            lifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<DepositProcessor>().Start();
            });
        }
    }
}