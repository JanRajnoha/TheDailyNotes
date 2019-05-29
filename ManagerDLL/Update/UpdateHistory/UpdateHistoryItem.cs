using System;

namespace Framework.Update.UpdateHistory
{
    /// <summary>
    /// Update history item structure
    /// </summary>
    public class UpdateHistoryItem<T>
    {
        public T Key { get; set; }
        public Version UpdateVersion { get; set; }
    }
}
