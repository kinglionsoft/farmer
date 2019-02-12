using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Logging;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.Hosting;

namespace GrpcTest
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new ConsoleLogger());
            if (args.Length == 0 || args[0] == "s")
            {
                await StartServer();
            }
            else
            {
                await Test(args[0], int.Parse(args[1]));
            }
        }

        static async Task StartServer()
        {
            var randomPort = new Random().Next(10000, 20000);
            var ports = new[] { new ServerPort("0.0.0.0", randomPort, ServerCredentials.Insecure) };
            using (var host = new HostBuilder()
                .UseMagicOnion(ports, new MagicOnion.Server.MagicOnionOptions(), types: new[] { typeof(TestServiceImpl) }).Build())
            {
                host.Start();
                Console.WriteLine($"listening on {randomPort}");

                await host.WaitForShutdownAsync();
            }
        }

        static async Task Test(string host, int randomPort)
        {
            var channel = new Channel(host, randomPort, ChannelCredentials.Insecure);
            var client = MagicOnion.Client.MagicOnionClient.Create<ITestService>(channel);
            for (int i = 0; i < 10; i++)
            {
                var ret = await client.Sum(i, i);
                Console.WriteLine($"{i} x 2 = {ret}");
            }
        }
    }
}
