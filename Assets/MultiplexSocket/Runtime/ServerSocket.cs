using System;
using Mirage.SocketLayer;

namespace Mirage.Sockets.Multiplex
{
    /// <summary>
    /// Socket that has multiple active sockets inside of it.
    /// </summary>
    internal class ServerSocket : ISocket
    {
        readonly ISocket[] sockets;
        ConnectionEndPoint readCache = new ConnectionEndPoint();

        public ServerSocket(ISocket[] sockets)
        {
            this.sockets = sockets;
        }

        public void Bind(IEndPoint _endPoint)
        {
            var endPoint = (BindEndPoint)_endPoint;
            for (int i = 0; i < sockets.Length; i++)
            {
                sockets[i].Bind(endPoint.EndPoints[i]);
            }
        }

        public void Close()
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                sockets[i].Close();
            }
        }

        public void Connect(IEndPoint endPoint)
        {
            throw new NotSupportedException();
        }

        public bool Poll()
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                bool hasPoll = sockets[i].Poll();
                if (hasPoll)
                    return true;
            }

            return false;
        }

        public int Receive(byte[] buffer, out IEndPoint endPoint)
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                if (sockets[i].Poll())
                {
                    int lng = sockets[i].Receive(buffer, out IEndPoint inner);
                    readCache.inner = inner;
                    readCache.socket = sockets[i];
                    endPoint = readCache;
                    return lng;
                }
            }

            throw new InvalidOperationException("Receive should not have been called if no socket has message");
        }

        public void Send(IEndPoint _endPoint, byte[] packet, int length)
        {
            var endPoint = (ConnectionEndPoint)_endPoint;
            endPoint.socket.Send(endPoint.inner, packet, length);
        }
    }
}
