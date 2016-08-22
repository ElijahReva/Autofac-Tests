namespace autofac_tests
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel.Configuration;

    public class PingService<TChecker, TService> : IPingService
        where TChecker : IPingProvider
    {
        private readonly Func<Uri, bool> action;

        private readonly Lazy<Uri> url;

        public PingService(TChecker checker)
        {
            this.action = checker.Ping;
            this.url = new Lazy<Uri>(() => this.GetServiceUriByServiceType(typeof(TService)));
        }

        public bool IsOnline()
        {
            //Console.WriteLine($"IsOnline by {typeof(TService)} pinged by [{typeof(TChecker)}]");
            return this.action(this.url.Value);
        }

        private Uri GetServiceUriByServiceType(Type t)
        {
            var client = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;


            var qasEndpoint = client?.Endpoints.Cast<ChannelEndpointElement>()
                .SingleOrDefault(endpoint => endpoint.Contract == t.ToString());
            return qasEndpoint?.Address;

        }     
    }
}