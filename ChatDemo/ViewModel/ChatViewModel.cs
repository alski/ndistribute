using ChatDemo.MVVM;
using nDistribute.WCF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nDistribute;

namespace ChatDemo.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        private WCFNetwork _network;
        private NetworkChannel<Message> _channel;

        public ChatViewModel()
        {
            User = Environment.UserName;
            _timerTask = new Task(timerMain, TaskCreationOptions.LongRunning);
            _timerTask.Start();

            _network = NetworkManager.Build();            
            _network.IsConnectedChanged += _network_IsConnectedChanged;
            _channel = _network.GetChannel<Message>();
            _channel.Received += _channel_Received;
            SendCommand = new Command { CanExecuteFunc = x => true, ExecuteAction = x => SendMessage() };
        }

        public void TryReconnect()
        {
            NetworkManager.TryConnect(_network);
        }

        void _channel_Received(object sender, Message e)
        {
            _conversation.Add(new MessageViewModel(e, false));
        }

        private void SendMessage()
        {
            var message = new Message(DateTime.UtcNow, User, Typing);
            _channel.Send(message);
            Typing = string.Empty;
            _conversation.Add(new MessageViewModel(message, true));
        }

        private void _network_IsConnectedChanged(object sender, EventArgs e)
        {
            IsConnected = _network.IsConnected;            
        }

        public void Connect()
        {
            try
            {
                _network.Connect(ConnectTo);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }


        private void timerMain()
        {
            do
            {
                var now = DateTime.Now;
                NotifyChanged("Now");

                Thread.Sleep(1000 - now.Millisecond);
            }
            while (true);
        }

        private ObservableCollection<MessageViewModel> _conversation = new ObservableCollection<MessageViewModel>();

        private string _user;
        private string _typing;
        private string _error;
        private Task _timerTask;
        private bool _isConnected;

        public ObservableCollection<MessageViewModel> Conversation { get { return _conversation; } }

        public string Now { get { return DateTime.Now.ToLongTimeString(); } }

        public string User
        {
            get { return _user; }
            set { SetAndNotifyChanged(ref _user, value); }
        }

        public string Typing
        {
            get { return _typing; }
            set { SetAndNotifyChanged(ref _typing, value); }
        }

        public string ThisConnection { get { return _network.Address.Address; } }

        public string ConnectTo { get; set; }

        public string Error
        {
            get { return _error; }
            private set { this.SetAndNotifyChanged(ref _error, value); }
        }

        public bool IsConnected
        {
            get { return this._isConnected; }
            set { SetAndNotifyChanged(ref _isConnected, value); }
        }

        public Command SendCommand { get; private set; }
    }
}
