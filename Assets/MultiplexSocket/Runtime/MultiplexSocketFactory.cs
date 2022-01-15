using System;
using System.Collections.Generic;
using Mirage.SocketLayer;
using UnityEngine;

namespace Mirage.Sockets.Multiplex
{
    /// <summary>
    /// Factory that creates all server sockets at once or first supported client socket
    /// </summary>
    public class MultiplexSocketFactory : SocketFactory
    {
        [Header("Sockets to use in order of priority")]
        public List<SocketFactory> Factories;

        public override ISocket CreateServerSocket()
        {
            ThrowIfNoFactories();

            var sockets = new ISocket[Factories.Count];
            for (int i = 0; i < Factories.Count; i++)
            {
                sockets[i] = Factories[i].CreateServerSocket();
            }

            return new ServerSocket(sockets);
        }

        public override IEndPoint GetBindEndPoint()
        {
            ThrowIfNoFactories();

            var endPoints = new IEndPoint[Factories.Count];
            for (int i = 0; i < Factories.Count; i++)
            {
                endPoints[i] = Factories[i].GetBindEndPoint();
            }

            return new BindEndPoint(endPoints);
        }

        public override ISocket CreateClientSocket()
        {
            ThrowIfNoFactories();

            // try each factory and return the first one that is supported
            // SocketFactory are expected to throw NotSupportedException when they are not supported for the current platform
            foreach (SocketFactory factory in Factories)
            {
                try
                {
                    return factory.CreateClientSocket();
                }
                catch (NotSupportedException) { }
            }

            throw new NotSupportedException("No Socket supported");
        }
        public override IEndPoint GetConnectEndPoint(string address = null, ushort? port = null)
        {
            ThrowIfNoFactories();

            // try each factory and return the first one that is supported
            // SocketFactory are expected to throw NotSupportedException when they are not supported for the current platform
            foreach (SocketFactory factory in Factories)
            {
                try
                {
                    return factory.GetConnectEndPoint(address, port);
                }
                catch (NotSupportedException) { }
            }

            throw new NotSupportedException("No Socket supported");
        }

        void ThrowIfNoFactories()
        {
            if (Factories.Count == 0)
                throw new InvalidOperationException("Factories list empty, add atleast 2 SocketFactory to list to use MultiplexSocketFactory correctly");
        }
    }
}
