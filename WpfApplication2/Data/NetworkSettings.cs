using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace Udp.Data
{
    internal sealed class NetworkSettings
    {
        internal const int MulticastPort = 5002;

        internal NetworkSettings()
        {
            PersonalAddress = GetMyAddress();
        }

        internal IPAddress PersonalAddress { get; }

        internal IPAddress MulticastAddress { get; private set; } = IPAddress.Parse("224.0.0.255");

        internal HashSet<IPAddress> Connected { get; } = new HashSet<IPAddress>();

        internal int ConnectedCount => Connected.Count;

        [NotNull]
        private static IPAddress GetMyAddress()
        {
            var ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            if (ipAddresses == null)
            {
                throw new Exception("Local IP Address Not Found!");
            }

            foreach (var ip in ipAddresses)
            {
                if (ip != null && ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        internal void TrySetMulticastAddress(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }

            if (string.Equals(userInput, "d", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            IPAddress address;

            if (!IPAddress.TryParse(userInput, out address))
            {
                return;
            }

            var major = address.GetAddressBytes().First();

            const int maxAddressMajor = 239;
            const int minAddressMajor = 224;

            if (major < minAddressMajor || major > maxAddressMajor)
            {
                return;
            }

            MulticastAddress = address;
        }
    }
}