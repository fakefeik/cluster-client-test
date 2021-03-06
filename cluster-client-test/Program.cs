﻿using System;
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


            Parallel.For(0, 2, i => clusterClient.Send(
                Request.Post("Search")
                    .WithContent(new byte[10])
                    .WithHeader("Content-Type", "application/x-grobuf"),
                TimeSpan.FromSeconds(10)
            ));
        }
    }
}