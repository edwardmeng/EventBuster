namespace EventBuster
{
    /// <summary>
    /// Specifies the priority of an event handler method.
    /// </summary>
    public enum HandlerPriority
    {
        /// <summary>
        /// The event handler method has highest priority.
        /// </summary>
        Highest = 0,

        /// <summary>
        /// The event handler method has high priority.
        /// </summary>
        High = 1,

        /// <summary>
        /// The event handler method has normal priority.
        /// </summary>
        Normal = 2,

        /// <summary>
        /// The event handler method has low priority.
        /// </summary>
        Low = 3,

        /// <summary>
        /// The event handler method has lowest priority.
        /// </summary>
        Lowest = 4
    }
}
