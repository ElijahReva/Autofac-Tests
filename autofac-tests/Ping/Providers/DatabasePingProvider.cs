namespace autofac_tests
{
    using System;

    public class DatabasePingProvider : IPingProvider
    {
        private static readonly Random rnd = new Random(Environment.TickCount);
        public Func<Uri, bool> Ping { get; } = uri =>
        {
            var next = rnd.NextBool(40);
            Console.WriteLine($"[DatabasePingProvider] - [{next}]");
            return next;
        };
    }
}