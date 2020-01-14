using System;

using MassTransit;
using MassTransit.Azure.ServiceBus.Core;

using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MTCommonContract;

namespace MTPublisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(serviceConfigurator =>
                    {
                        serviceConfigurator.AddBus(provider =>
                            Bus.Factory.CreateUsingAzureServiceBus(busConfiguration =>
                            {
                                IServiceBusHost host = busConfiguration.Host(new Uri("sb://lennartazuretest.servicebus.windows.net/"), (sbCfg) =>
                                {
                                    sbCfg.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey", "HUFMOfa23zFs+nt43t0enAdKnDBObcJOEfsqj/cqPoc=");
                                });

                                busConfiguration.Message<IContract>(topology =>
                                    topology.SetEntityName("mtcommoncontract/icontract"));
                            }));
                        services.AddHostedService<Worker>();
                    });
                });
    }
}
