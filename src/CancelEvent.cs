namespace EventBuster
{
    /// <summary>
    /// Provides data for a cancelable event.
    /// </summary>
    public class CancelEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEvent"/> class with the <see cref="Cancel"/> property set to false.
        /// </summary>
        public CancelEvent()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEvent"/> class with the <see cref="Cancel"/> property set to the given value.
        /// </summary>
        /// <param name="cancel"><c>true</c> to cancel the event; otherwise, <c>false</c>.</param>
        public CancelEvent(bool cancel)
        {
            Cancel = cancel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        /// <value><c>true</c> if the event should be canceled; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }
}
