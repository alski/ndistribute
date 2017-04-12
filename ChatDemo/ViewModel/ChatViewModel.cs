namespace ChatDemo.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using demoMVVM;
    using nDistribute;
    using nDistribute.WCF;

    /// <summary>
    /// A viewModel for chat
    /// </summary>
    public class ChatViewModel : ViewModelBase
    {
        private WCFNetwork network;
        private NetworkChannel<Message> channel;
        private string user;
        private string typing;
        private string error;
        private Task timerTask;
        private bool isConnected;
        private string now;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatViewModel"/> class.
        /// </summary>
        public ChatViewModel()
        {
            User = Environment.UserName;
            timerTask = new Task(TimerMain, TaskCreationOptions.LongRunning);
            timerTask.Start();

            network = WCFNetworkManager.Build();
            network.IsConnectedChanged += NetworkIsConnectedChanged;
            channel = network.GetChannel<Message>();
            channel.Received += ChannelReceived;
            SendCommand = new Command { CanExecuteFunc = x => true, ExecuteAction = x => SendMessage() };
        }

        /// <summary>
        /// Gets the collection of <see cref="MessageViewModel"/> that makes a conversation
        /// </summary>
        public ObservableCollection<MessageViewModel> Conversation { get; } = new ObservableCollection<MessageViewModel>();

        /// <summary>
        /// Gets or sets current time. Used so we can show dynamic behaviour in the demo,.
        /// </summary>
        public string Now
        {
            get { return now; }
            set { SetAndNotifyChanged(ref now, value); }
        }

        /// <summary>
        /// Gets or sets and sets the user.
        /// </summary>
        public string User
        {
            get { return user; }
            set { SetAndNotifyChanged(ref user, value); }
        }

        /// <summary>
        /// Gets or sets the current set of characters not yet sent as a message.
        /// </summary>
        public string Typing
        {
            get { return typing; }
            set { SetAndNotifyChanged(ref typing, value); }
        }

        /// <summary>
        /// Gets the connection as an address of the local node on the network.
        /// </summary>
        public string ThisConnection => network.Local.Address.ToString();

        /// <summary>
        /// Gets or sets 
        /// </summary>
        public string ConnectTo { get; set; }

        /// <summary>
        /// Gets and sets the current error state.
        /// </summary>
        public string Error
        {
            get { return error; }
            private set { SetAndNotifyChanged(ref error, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is connected to the network.
        /// </summary>
        public bool IsConnected
        {
            get { return isConnected; }
            set { SetAndNotifyChanged(ref isConnected, value); }
        }

        /// <summary>
        /// Gets the command to Send the <see cref="Message"/> to the network.
        /// </summary>
        public Command SendCommand { get; private set; }

        /// <summary>
        /// Connects to the network or sets <see cref="Error"/> with the reason why not.
        /// </summary>
        public void Connect()
        {
            try
            {
                network.Connect(new NodeAddress(ConnectTo));
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        /// <summary>
        /// Try to reconnect
        /// </summary>
        public void TryReconnect()
        {
            WCFNetworkManager.TryConnect(network);
        }

        private void ChannelReceived(object sender, Message e)
        {
            Conversation.Add(new MessageViewModel(e, false));
        }

        private void SendMessage()
        {
            var message = new Message(DateTime.UtcNow, User, Typing);
            channel.Send(message);
            Typing = string.Empty;
            Conversation.Add(new MessageViewModel(message, true));
        }

        private void NetworkIsConnectedChanged(object sender, EventArgs e)
        {
            IsConnected = network.IsConnected;            
        }

        private void TimerMain()
        {
            do
            {
                Now = DateTime.Now.ToLongTimeString();
                
                Thread.Sleep(1000 - DateTime.Now.Millisecond);
            }
            while (true);
        }      
    }
}
