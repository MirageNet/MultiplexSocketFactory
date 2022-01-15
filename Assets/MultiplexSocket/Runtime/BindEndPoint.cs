using System;
using Mirage.SocketLayer;

namespace Mirage.Sockets.Multiplex
{
    /// <summary>
    /// Endpoint that contains multiple endpoints, should only be used to Bind <see cref="ServerSocket"/>
    /// </summary>
    internal class BindEndPoint : IEndPoint
    {
        public readonly IEndPoint[] EndPoints;

        public BindEndPoint(IEndPoint[] endPoints)
        {
            EndPoints = endPoints;
        }

        public IEndPoint CreateCopy()
        {
            throw new NotSupportedException("Cant create copy for bind endpoint");
        }
    }
}
