using System;
using System.Windows;

namespace Udp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                using (var model = new Model())
                {
                    using (var viewModel = new ViewModel.ViewModel(model))
                    {
                        var mainWindow = new MainWindow(viewModel);

                        mainWindow.ShowDialog();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}