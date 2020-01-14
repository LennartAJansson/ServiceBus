using System;
using System.Diagnostics;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MTQueueTest
{

    [TestClass]
    public static class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            Trace.WriteLine("AssemblyInitialize");
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Trace.WriteLine("AssemblyCleanup");
        }
    }

    [TestClass]
    public class UnitTest1
    {
        static IServiceCollection serviceCollection;
        static IServiceProvider serviceProvider;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<MyConsumer>();
            serviceCollection.AddMassTransit(serviceConfigurator =>
            {
                serviceConfigurator.AddConsumer<MyConsumer>();
                serviceConfigurator.AddBus(provider =>
                    Bus.Factory.CreateUsingInMemory(busConfiguration =>
                    {
                        busConfiguration.ReceiveEndpoint("pelle", ep =>
                        {
                            ep.ConfigureConsumer<MyConsumer>(provider);
                        });
                    }));
            });

            serviceCollection.AddLogging();

            serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<UnitTest1>();

            logger.LogDebug("ClassInitialize");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Debug.WriteLine("ClassCleanup");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Trace.WriteLine("TestInitialize");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Trace.WriteLine("TestCleanup");
        }

        [TestMethod]
        public void TestMethod1()
        {

            Trace.WriteLine("TestMethod1");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Trace.WriteLine("TestMethod2");
            Assert.IsTrue(true);
        }
    }

    internal class MyConsumer : IConsumer<IContract>
    {
        public Task Consume(ConsumeContext<IContract> context) => throw new NotImplementedException();
    }

    internal interface IContract
    {
        string Text { get; set; }
    }

    internal interface IContractv2 : IContract
    {
        string OtherText { get; set; }
    }

    public class MyClass : IContractv2
    {
        public string Text { get; set; }
        public string OtherText { get; set; }
    }

}
