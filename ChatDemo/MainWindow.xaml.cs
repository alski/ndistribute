namespace ChatDemo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ChatDemo.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter
                && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                SendButton.Focus();
                ((ChatViewModel)SendButton.DataContext).SendCommand.Execute(null);
                ((TextBox)sender).Focus();
            }
        } 
    }
}
