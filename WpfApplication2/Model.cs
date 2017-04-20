using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Udp.Data;

namespace Udp
{
    internal sealed class Model : Disposable
    {
        [NotNull] private readonly MulticastReceiver _multicastReceiver;
        [NotNull] private readonly NetworkSettings _networkSettings = new NetworkSettings();
        [NotNull] private readonly PersonalAddressHandler _personalAddressHandler;

        [NotNull] private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _disposed;

        private bool _enabled;
        private Thread _listeningThread;

        internal Model()
        {
            _personalAddressHandler = new PersonalAddressHandler(_networkSettings);

            _multicastReceiver = new MulticastReceiver(_networkSettings)
            {
                CancellationToken = _cancellationTokenSource.Token
            };
        }

        internal string PersonalAddress => _networkSettings.PersonalAddress.ToString();

        internal string MulticastAddress
        {
            get { return _networkSettings.MulticastAddress.ToString(); }
            set { _networkSettings.TrySetMulticastAddress(value); }
        }

        internal int ActiveClientCount => _networkSettings.ConnectedCount;

        internal void Stop()
        {
            Task.Run(() => _personalAddressHandler.SayBye());
            _personalAddressHandler.StopSending();

            _networkSettings.Connected.Clear();

            _cancellationTokenSource.Cancel();
            try
            {
                _listeningThread.Join();
            }
            catch
            {
                // ignored
            }
        }

        internal void Start()
        {
            ResetConnected();

            _personalAddressHandler.SayHello();
            _personalAddressHandler.StartSending();

            _cancellationTokenSource = new CancellationTokenSource();

            _multicastReceiver.CancellationToken = _cancellationTokenSource.Token;

            _listeningThread = new Thread(_multicastReceiver.StartListen);
            _listeningThread.Start();

            _enabled = true;
        }

        internal void ClearConnected()
        {
            _networkSettings.Connected.Clear();
        }

        private void ResetConnected()
        {
            _networkSettings.Connected.Clear();
            _networkSettings.Connected.Add(_networkSettings.PersonalAddress);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_enabled)
                {
                    Stop();
                }

                _multicastReceiver.Dispose();
                _personalAddressHandler.Dispose();

            }

            _disposed = true;
        }
    }
}