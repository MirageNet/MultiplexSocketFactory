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

        public override bool Equals(object obj)
        {
            if (obj is ConnectionEndPoint other)
            {
                return socket.Equals(other.socket) && inner.Equals(other.inner);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + socket.GetHashCode();
            hash = hash * 23 + inner.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"Socket:{socket} EndPoint{inner}";
        }
    }
}
