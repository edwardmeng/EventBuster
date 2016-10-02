namespace EventBuster
{
    /// <summary>
    /// An enumeration that is used with the <see cref="EventHandlerAttribute"/> to specify the transaction flow policy.
    /// </summary>
    public enum TransactionFlowOption
    {
        /// <summary>
        /// Transaction may be flowed.
        /// </summary>
        NotAllowed,

        /// <summary>
        /// Transaction may be flowed.
        /// </summary>
        Allowed,

        /// <summary>
        /// Transaction must be flowed.
        /// </summary>
        Mandatory
    }
}
