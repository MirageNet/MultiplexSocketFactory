using Mirage.SocketLayer;

namespace Mirage.Sockets.Multiplex
{
    /// <summary>
    /// Endpoint that has another endpoint and the socket it belongs to
    /// </summary>
    internal class ConnectionEndPoint : IEndPoint
    {
        public IEndPoint inner;
        public ISocket socket;

        public IEndPoint CreateCopy()
        {
            return new ConnectionEndPoint()
            {
                inner = inner.CreateCopy(),
                socket = socket
            };
        }
    }
}
