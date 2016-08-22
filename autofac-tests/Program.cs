namespace autofac_tests
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;

    using Autofac;

    public class Program
    {
        private static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var builder = new ContainerBuilder();
            builder.RegisterType<PingService<HttpPingProvider, IFoo>>()
                .Keyed<IPingService>(Checker.Foo);
            builder.RegisterType<PingService<DatabasePingProvider, IBar>>()
                .Keyed<IPingService>(Checker.Bar);
            builder.RegisterType<DatabasePingProvider>();
            builder.RegisterType<NetTcpPingProvider>();
            builder.RegisterType<HttpPingProvider>();
            builder.RegisterType<StatusManager>(); 
            var container = builder.Build();

            sw.Stop();
            Console.WriteLine($"Loaded. Takes {sw.ElapsedMilliseconds} ms!");

            var sm = container.Resolve<StatusManager>(); 
            sm.Start();
            sm.EventQueue
                .GetConsumingEnumerable()
                .ToObservable()
                .Subscribe(Sm_StatusChanged);
            sm.GetCurrentStatus(Checker.Foo);
            sm.GetCurrentStatus(Checker.Bar);
            Console.ReadLine();

        }

        private static void Sm_StatusChanged(CheckerEventArgs newRes)
        {
            Console.WriteLine($"[{newRes.Status}] - [{newRes.Checker}]");
        }
    }

    public static class RandomExtention
    {
        public static bool NextBool(this Random r, int truePercentage = 50)
        {
            return r.NextDouble() < truePercentage / 100.0;
        }
    }

    interface IFoo
    {
         
    }

    interface IBar
    {
         
    }

    public enum Checker
    {
        Bar,
        Foo
    }
}
