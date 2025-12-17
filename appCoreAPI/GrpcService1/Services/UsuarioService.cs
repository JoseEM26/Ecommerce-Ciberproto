using Grpc.Core;
using System.Data;
using GrpcService1;

namespace GrpcService1.Services
{
    public class UsuarioService : Usuario.UsuarioBase
    {
        public override Task<HelloReply> SayHello(
            HelloRequest request,
            ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = $"Hola {request.Name}"
            });
        }
    }
}
