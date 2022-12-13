using UnityEngine;
using Grpc.Core;
using Grpc.Sample;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HelloClient
{

    public void Hello()
    {
        Channel channel = new Channel("localhost:8080", ChannelCredentials.Insecure);

        var client = new GreetingService.GreetingServiceClient(channel);

        var replay = client.Hello(new HelloRequest { Name = "Hoge" });

        channel.ShutdownAsync().Wait();

        Debug.Log(replay.Message);
    }

    public async Task HelloServerStream()
    {
        Channel channel = new Channel("localhost:8080", ChannelCredentials.Insecure);

        var client = new GreetingService.GreetingServiceClient(channel);

        var helloRequest = new HelloRequest() { Name = "Hoge" };
        var replay = client.HelloServerStream(helloRequest);

        while (await replay.ResponseStream.MoveNext())
        {
            var curreantReplay = replay.ResponseStream.Current;
            Debug.Log(curreantReplay.Message);
        }

        channel.ShutdownAsync().Wait();
    }

    public async Task HelloClientStream()
    {
        Channel channel = new Channel("localhost:8080", ChannelCredentials.Insecure);

        var client = new GreetingService.GreetingServiceClient(channel);

        var helloRequests = new List<HelloRequest>()
        {
            new HelloRequest {Name = "a"}, new HelloRequest {Name = "b"},
            new HelloRequest {Name = "c"}, new HelloRequest {Name = "d"}, new HelloRequest {Name = "e"},
        };

        using var replay = client.HelloClientStream();

        for (var i = 0; i < 5; i++)
        {
            await replay.RequestStream.WriteAsync(new HelloRequest { Name = helloRequests[i].Name });
        }
        await replay.RequestStream.CompleteAsync();

        var response = await replay;
        Debug.Log(response.Message);

        channel.ShutdownAsync().Wait();
    }

    public async Task HelloBiStreams()
    {
        Channel channel = new Channel("localhost:8080", ChannelCredentials.Insecure);

        var client = new GreetingService.GreetingServiceClient(channel);

        using var replay = client.HelloBiStreams();

        var responseTask = Task.Run(async () =>
        {
            while (await replay.ResponseStream.MoveNext())
            {
                // �󂯎��
                Debug.Log(replay.ResponseStream.Current);
            }
        });

        var helloRequests = new List<HelloRequest>()
        {
            new HelloRequest {Name = "a"}, new HelloRequest {Name = "b"},
            new HelloRequest {Name = "c"}, new HelloRequest {Name = "d"}, new HelloRequest {Name = "e"},
        };
        for (var i = 0; i < 5; i++)
        {
            await replay.RequestStream.WriteAsync(new HelloRequest { Name = helloRequests[i].Name });
        }
        await replay.RequestStream.CompleteAsync();
    }
}
