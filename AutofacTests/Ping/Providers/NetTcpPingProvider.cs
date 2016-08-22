namespace AutofacTests.Ping.Providers
{
    using System;

    public class NetTcpPingProvider : IPingProvider
    {
        private static readonly Random rnd = new Random(Environment.TickCount);
        public Func<Uri, bool> Ping { get; } = uri =>
            {
                var next = rnd.NextBool(40);
                Console.WriteLine($"[NetTcpPingProvider] -          [{next}]");
                return next;
            };
    }
}