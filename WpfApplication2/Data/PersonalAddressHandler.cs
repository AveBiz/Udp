using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using JetBrains.Annotations;

namespace Udp.Data
{
    internal sealed class PersonalAddressHandler : Disposable
    {
        [NotNull] private readonly NetworkSettings _networkSettings;
        [NotNull] private readonly UdpClient _personalAddress;
        [NotNull] private readonly Timer _timer = new Timer();

        private bool _disposed;

        internal PersonalAddressHandler([NotNull] NetworkSettings networkSettings)
        {
            if (networkSettings == null)
            {
                throw new ArgumentNullException(nameof(networkSettings));
            }

            _networkSettings = networkSettings;
            try
            {
                _personalAddress = new UdpClient();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw;
            }

            InitializeTimer();
        }

        internal void StopSending()
        {
            if (_disposed)
            {
                return;
            }

            _timer.Stop();
        }

        private void InitializeTimer()
        {
            const int timerInterval = 5000;

            _timer.Interval = timerInterval;
            _timer.Elapsed += OnTimerElapsed;
        }

        internal void StartSending()
        {
            if (_disposed)
            {
                return;
            }

            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SayHello();
        }

        internal async void SayBye()
        {
            try
            {
                var message = StatusCodes.Bye.ToString();
                await SendWord(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        internal async void SayHello()
        {
            try
            {
                var message = StatusCodes.Hello.ToString();
                await SendWord(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task SendWord([NotNull] string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var address = new IPEndPoint(_networkSettings.MulticastAddress, NetworkSettings.MulticastPort);

            var bytes = Encoding.Unicode.GetBytes(message);

            await _personalAddress.SendAsync(bytes, bytes.Length, address);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                StopSending();

                _timer.Dispose();

                _personalAddress.Close();
                _personalAddress.Dispose();
            }

            _disposed = true;
        }
    }
}