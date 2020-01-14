using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MTSyncQueueListener
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
                    services.AddScoped<MyConsumer>();

                    services.AddMassTransit(serviceConfigurator =>
                    {
                        serviceConfigurator.AddConsumer<MyConsumer>();
                        serviceConfigurator.AddBus(provider =>
                            Bus.Factory.CreateUsingInMemory(busConfiguration =>
                            //Bus.Factory.CreateUsingAzureServiceBus(busConfiguration =>
                            {
                                //IServiceBusHost host = busConfiguration.Host(new Uri("sb://lennartazuretest.servicebus.windows.net/"), (sbCfg) =>
                                //{
                                //    sbCfg.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey", "HUFMOfa23zFs+nt43t0enAdKnDBObcJOEfsqj/cqPoc=");
                                //});

                                busConfiguration.ReceiveEndpoint("pelle", ep =>
                                {
                                    //ep.PrefetchCount = 4;
                                    ep.ConfigureConsumer<MyConsumer>(provider);
                                });
                            }));
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
