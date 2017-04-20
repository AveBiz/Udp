using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Udp.Data
{
    internal sealed class MulticastReceiver : Disposable
    {
        [NotNull] private readonly UdpClient _multicastReceiver;
        [NotNull] private readonly NetworkSettings _networkSettings;

        internal CancellationToken CancellationToken { private get; set; }

        private bool _disposed;

        internal MulticastReceiver([NotNull] NetworkSettings networkSettings)
        {
            if (networkSettings == null)
            {
                throw new ArgumentNullException(nameof(networkSettings));
            }

            _networkSettings = networkSettings;
            _multicastReceiver = new UdpClient();
            _multicastReceiver = new UdpClient(NetworkSettings.MulticastPort);

            JoinGroup();
        }

        internal void StartListen()
        {
            if (_disposed)
            {
                return;
            }

            _multicastReceiver.Client.ReceiveTimeout = 500;
            while (!CancellationToken.IsCancellationRequested)
            {
                IPEndPoint ipEndPoint = null;
                byte[] receivedBytes;

                try
                {
                    receivedBytes = _multicastReceiver.Receive(ref ipEndPoint);
                }
                catch (SocketException)
                {
                    continue;
                }

                if (Equals(ipEndPoint.Address, _networkSettings.PersonalAddress))
                {
                    continue;
                }

                var receivedData = Encoding.Unicode.GetString(receivedBytes);

                StatusCodes receivedStatusCode;
                if (!Enum.TryParse(receivedData, true, out receivedStatusCode))
                {
                    continue;
                }

                if (receivedStatusCode == StatusCodes.Hello)
                {
                    _networkSettings.Connected.Add(ipEndPoint.Address);
                }

                if (receivedStatusCode == StatusCodes.Bye)
                {
                    _networkSettings.Connected.Remove(ipEndPoint.Address);
                }
            }
        }

        private void JoinGroup()
        {
            _multicastReceiver.JoinMulticastGroup(_networkSettings.MulticastAddress);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _multicastReceiver.DropMulticastGroup(_networkSettings.MulticastAddress);
                _multicastReceiver.Close();
                _multicastReceiver.Dispose();
            }

            _disposed = true;
        }
    }
}