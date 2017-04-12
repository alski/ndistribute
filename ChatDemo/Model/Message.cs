namespace ChatDemo.ViewModel
{
    using System;

    /// <summary>
    /// An example message format.
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="whenUtc">Time in UTC that the message was first sent</param>
        /// <param name="user">User that sent the <see cref="Message"/></param>
        /// <param name="message">Actual testual content</param>
        public Message(DateTime whenUtc, string user, string message)
        {
            WhenUTC = whenUtc;
            User = user;
            Text = message;
        }

        /// <summary>
        /// Gets the time that the message was created.
        /// </summary>
        public DateTime WhenUTC { get; private set; }

        /// <summary>
        /// Gets the user that created the message.
        /// </summary>
        public string User { get; private set; }

        /// <summary>
        /// Gets the actual textual content of the message.
        /// </summary>
        public string Text { get; private set; }
    }
}
