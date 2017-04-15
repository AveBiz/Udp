using System;
using JetBrains.Annotations;

namespace Udp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal sealed class MainWindow : Window
    {
        private MainWindow()
        {
            InitializeComponent();
        }

        internal MainWindow([NotNull] ViewModel.ViewModel viewModel) : this()
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            DataContext = viewModel;
        }
    }
}