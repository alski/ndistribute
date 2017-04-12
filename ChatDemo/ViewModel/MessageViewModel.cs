namespace ChatDemo.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// A viewmodel for Messages to be displayed.
    /// </summary>
    [Serializable]
    public class MessageViewModel
    {
        private Message message;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageViewModel"/> class.
        /// </summary>
        /// <param name="message">The message model to wrap in this ViewModel</param>
        /// <param name="isLocal">Indication that the message was created locally.</param>
        public MessageViewModel(Message message, bool isLocal)
        {
            this.message = message;
            TextBrush = isLocal ? SystemColors.WindowTextBrush : SystemColors.GrayTextBrush;
        }

        /// <summary>
        /// Gets the time that message was created.
        /// </summary>
        public DateTime WhenUTC => message.WhenUTC;

        /// <summary>
        /// Gets the user that created the message.
        /// </summary>
        public string User => message.User;
        
        /// <summary>
        /// Gets the mesage.
        /// </summary>
        public string Message => message.Text;
        
        /// <summary>
        /// Gets the Brush that should be used to render the message.
        /// </summary>
        public Brush TextBrush { get; private set; }
    }
}
