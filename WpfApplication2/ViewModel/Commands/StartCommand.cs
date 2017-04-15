using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Udp.ViewModel.Commands
{
    internal sealed class StartCommand : ICommand
    {
        [NotNull] private readonly ViewModel _viewModel;

        internal StartCommand([NotNull] ViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.Start();
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}