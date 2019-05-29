using System.Collections.Generic;

namespace Framework.Update.UpdateHistory
{
    /// <summary>
    /// Update history file structure
    /// </summary>
    public class UpdateHistoryFile
    {
        public string LastVersion { get; set; }
        public List<UpdateItem> UpdateList { get; set; }
    }
}
