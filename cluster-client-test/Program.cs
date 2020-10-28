using System;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Criteria;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Ordering.Weighed;
using Vostok.Clusterclient.Core.Ordering.Weighed.Adaptive;
using Vostok.Clusterclient.Core.Strategies;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Transport;
using Vostok.Logging.Abstractions;

namespace cluster_client_test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var clusterClient = new ClusterClient(new SilentLog(), configuration =>
            {
                configuration.SetupUniversalTransport(new UniversalTransportSettings
                {
                    AllowAutoRedirect = false,
                    TcpKeepAliveEnabled = true,
                    MaxConnectionsPerEndpoint = 4096,
                });
                configuration.DefaultConnectionTimeout = TimeSpan.FromMilliseconds(750);
                configuration.DefaultTimeout = TimeSpan.FromSeconds(10);
                configuration.ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:8086"));
                configuration.DefaultRequestStrategy = Strategy.Sequential1;
            });


            var request = Request.Post("Search")
                .WithContent(new byte[10])
                .WithHeader("Content-Type", "application/x-grobuf");
            Parallel.For(0, 10, i =>
                Task.WaitAll(Enumerable.Range(0, 100_000).Select(async x =>
                {
                    await clusterClient.SendAsync(request, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                }).ToArray()));
        }
    }
}