using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using Helloworld;

namespace greeterapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RpcController : ControllerBase
    {
        private Greeter.GreeterClient Client
        {
            get
            {
                Channel channel = new Channel("greeter-server:50051", ChannelCredentials.Insecure);
                return new Greeter.GreeterClient(channel);
            }
        }

        public ActionResult<string> Hello(string u)
        {
            try
            {
                var reply = Client.SayHello(new HelloRequest { Name = u });

                return reply.Message;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public async Task<ActionResult<string>> Stream()
        {
            using (var call = Client.Send())
            {
                var responseReaderTask = Task.Run<FileReply>(async () =>
                {
                    while (await call.ResponseStream.MoveNext(default(CancellationToken)))
                    {
                        var reply1 = call.ResponseStream.Current;
                        return reply1;
                    }
                    return null;
                });

                await call.RequestStream.WriteAsync(new FileRequest
                {
                    Name = "file1",
                    Data = Google.Protobuf.ByteString.CopyFromUtf8("yangchao")
                });
                await call.RequestStream.CompleteAsync();
                var reply = await responseReaderTask;
                return ("Received: " + reply.Name + ", " + reply.Data.ToStringUtf8());
            }
        }

        public ActionResult<string> Test()
        {
            return "ok";
        }

    }
}
