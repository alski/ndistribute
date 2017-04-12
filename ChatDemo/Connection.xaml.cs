namespace ChatDemo
{
    using System.Windows;
    using ChatDemo.ViewModel;

    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    public partial class Connection : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        public Connection()
        {
            InitializeComponent();

            var chat = new ChatViewModel();
            chat.PropertyChanged += ChatPropertyChanged;
            DataContext = chat;
            chat.TryReconnect();
        }

        private void ChatPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
