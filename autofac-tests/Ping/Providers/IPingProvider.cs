namespace autofac_tests
{
    using System;

    public interface IPingProvider
    {
        Func<Uri, bool> Ping { get; }
    }
}