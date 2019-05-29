using Framework.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Modules.ToDos.Classes
{
    public class ToDo : TaskBaseItem
    {
        // Commands
        [JsonIgnore]
        [XmlIgnore]
        public ICommand Complete { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICommand Remove { get; set; }

        /// <summary>
        /// Init Activity item class
        /// </summary>
        public ToDo()
        {
            Dates = new ObservableCollection<DateTime>();

            Complete = new CompletedButtonEvent();
            Remove = new DeleteButtonEvent();
        }

        /// <summary>
        /// Init Activity item class
        /// </summary>
        /// <param name="detailActivity">Default values</param>
        public ToDo(ToDo detailActivity)
        {
            ID = detailActivity.ID;
            Name = detailActivity.Name;
            Secured = detailActivity.Secured;
            Description = detailActivity.Description;
            Start = detailActivity.Start;
            End = detailActivity.End;
            Neverend = detailActivity.Neverend;
            WhenNotify = detailActivity.WhenNotify;
            Notify = detailActivity.Notify;
            NotifyDays = detailActivity.NotifyDays;
            Dates = detailActivity.Dates;

            Complete = detailActivity.Complete;
            Remove = detailActivity.Remove;
        }

        public override object Clone()
        {
            return new ToDo(this);
        }
    }
}
