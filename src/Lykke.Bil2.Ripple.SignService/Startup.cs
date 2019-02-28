using System;
using JetBrains.Annotations;
using Lykke.Bil2.Ripple.SignService.Services;
using Lykke.Bil2.Ripple.SignService.Settings;
using Lykke.Bil2.Sdk.SignService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Ripple.SignService
{
    [UsedImplicitly]
    public class Startup
    {
        private const string IntegrationName = "Ripple";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainSignServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = IntegrationName;

                // Register required service implementations:

                options.TransactionSignerFactory = ctx =>
                    new TransactionSigner();

                options.AddressGeneratorFactory = ctx =>
                    new AddressGenerator();
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainSignService(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
