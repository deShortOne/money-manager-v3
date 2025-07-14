
namespace MoneyTracker.Common.Values;
public enum ReceiptState
{
    Unknown = 0,
    /// <summary>
    /// Receipt currently being analysed
    /// </summary>
    Processing = 1,
    /// <summary>
    /// All receipt stuff has been done
    /// </summary>
    Finished = 2,
    /// <summary>
    /// Error with processing receipt state
    /// </summary>
    Error = 3,
    /// <summary>
    /// Processing finished and waiting for receipt data to become transaction
    /// </summary>
    Pending = 4,
}
