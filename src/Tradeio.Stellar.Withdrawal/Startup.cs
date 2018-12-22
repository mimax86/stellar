using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using Tradeio.Balance;
using Tradeio.Email;
using Tradeio.Stellar.Configuration;
using Tradeio.Stellar.Data;
using Tradeio.Stellar.Processors;
using Tradeio.Stellar.Withdrawal.Controllers;

namespace Tradeio.Stellar.Withdrawal
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

            services.AddTransient<IStellarService, StellarService>();

            services.AddTransient<IStellarConfigurationService, StellarConfigurationService>();
            services.AddTransient<IStellarRepository, StellarRepository>();

            services.AddTransient<IWithdrawalsController, WithdrawalsControllerInternal>();
            services.AddTransient<IStatusController, StatusControllerInternal>();

            services.AddSingleton<WithdrawalProcessor>();
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
                settings.GeneratorSettings.Title = "Stellar Withdrawal API";
                settings.GeneratorSettings.Description = "";
            });

            app.UseSwaggerReDocWithApiExplorer(settings => settings.SwaggerUiRoute = "/docs");

            lifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<WithdrawalProcessor>().Start();
            });
        }
    }
}