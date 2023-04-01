using System;
using System.Collections.Generic;
using System.Linq;
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

        // once we find a SocketFactory that works on client, we want to keep using it
        private SocketFactory _clientFactory;

        // pick lowest size, so that mirage can be configured to work with any of them
        public override int MaxPacketSize => Factories.Min(x => x.MaxPacketSize);

        public override ISocket CreateServerSocket()
        {
            ThrowIfNoFactories();

            // server needs to listen on all sockets
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

            // server needs to listen on all sockets
            var endPoints = new IEndPoint[Factories.Count];
            for (int i = 0; i < Factories.Count; i++)
            {
                endPoints[i] = Factories[i].GetBindEndPoint();
            }

            return new BindEndPoint(endPoints);
        }

        private SocketFactory FindSupportedFactory()
        {
            // try each factory and return the first one that is supported
            // SocketFactory are expected to throw NotSupportedException when they are not supported for the current platform
            foreach (SocketFactory factory in Factories)
            {
                try
                {
                    // see if factory is supported by creating a socket
                    // socket should not do anything until bind/connect is called, so we can just discard it
                    _ = factory.CreateClientSocket();
                    // we found a socket that works, store it so we can use it later
                    return factory;
                }
                catch (NotSupportedException) { }
            }

            throw new NotSupportedException("No Socket supported");
        }

        public override ISocket CreateClientSocket()
        {
            ThrowIfNoFactories();

            // if we have already found a good factory, keep using it
            if (_clientFactory == null)
                _clientFactory = FindSupportedFactory();

            return _clientFactory.CreateClientSocket();


        }
        public override IEndPoint GetConnectEndPoint(string address = null, ushort? port = null)
        {
            ThrowIfNoFactories();

            // if we have already found a good factory, keep using it
            if (_clientFactory == null)
                _clientFactory = FindSupportedFactory();

            return _clientFactory.GetConnectEndPoint(address, port);
        }

        private void ThrowIfNoFactories()
        {
            if (Factories.Count == 0)
                throw new InvalidOperationException("Factories list empty, add atleast 2 SocketFactory to list to use MultiplexSocketFactory correctly");
        }
    }
}
