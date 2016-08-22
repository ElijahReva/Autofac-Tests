namespace AutofacTests.Ping.Providers
{
    using System;

    public interface IPingProvider
    {
        Func<Uri, bool> Ping { get; }
    }
}