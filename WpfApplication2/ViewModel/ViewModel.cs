using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using JetBrains.Annotations;
using Udp.ViewModel.Commands;

namespace Udp.ViewModel
{
    internal sealed class ViewModel : Disposable, INotifyPropertyChanged
    {
        private readonly Timer _activeClientsChangedTimer = new Timer(200);

        [NotNull] private readonly Model _model;

        private bool _disposed;
        private bool _isActive;

        internal ViewModel([NotNull] Model model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            _model = model;

            StartCommand = new StartCommand(this);
            StopCommand = new StopCommand(this);
            InitializeTimer();
        }

        public string MulticastAddress
        {
            get { return _model.MulticastAddress; }
            set
            {
                _model.MulticastAddress = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public int ActiveClientCount => _model.ActiveClientCount;

        public ICommand StartCommand { get; }

        public ICommand StopCommand { get; }

        public string PersonalAddressText => $"Ваш Ip адрес: {_model.PersonalAddress}";

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeTimer()
        {
            _activeClientsChangedTimer.Elapsed += OnActiveClientsChangedTimerElapsed;
        }

        private void OnActiveClientsChangedTimerElapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(ActiveClientCount));
        }

        internal void Stop()
        {
            _activeClientsChangedTimer.Stop();
            
            IsActive = false;

            _model.Stop();
            _model.ClearConnected();

            OnPropertyChanged(nameof(ActiveClientCount));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Start()
        {
            _model.Start();

            IsActive = true;

            _activeClientsChangedTimer.Start();

            OnPropertyChanged(nameof(ActiveClientCount));
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _activeClientsChangedTimer.Stop();
            _activeClientsChangedTimer.Dispose();

            _model.Dispose();

            _disposed = true;
        }
    }
}