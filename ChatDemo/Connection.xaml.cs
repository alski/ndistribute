using ChatDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatDemo
{
    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    public partial class Connection : Window
    {
        public Connection()
        {
            InitializeComponent();

            var chat = new ChatViewModel();
            DataContext = chat;
            chat.PropertyChanged += chat_PropertyChanged;
            chat.TryReconnect();
        }

        void chat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsConnected")
            {
                var chat = (ChatViewModel)DataContext;
                if (chat.IsConnected)
                {
                    new MainWindow { DataContext = chat }.Show();
                    Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TryConnect();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            TryConnect();
        }

        private void TryConnect()
        {
            try
            {
                UserInput.Visibility = System.Windows.Visibility.Collapsed;
                connecting.Visibility = System.Windows.Visibility.Visible;

                var chat = (ChatViewModel)DataContext;
                if (string.IsNullOrWhiteSpace(chat.ConnectTo) == false)
                {
                    chat.Connect();
                }
            }
            finally
            {
                UserInput.Visibility = System.Windows.Visibility.Visible;
                connecting.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
