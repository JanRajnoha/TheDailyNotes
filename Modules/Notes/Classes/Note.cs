using Framework.Template;
using Newtonsoft.Json;
using System;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Modules.Notes.Classes
{
    /// <summary>
    /// Note item class
    /// </summary>
    public class Note : BaseItem
    {
        public DateTime Created { get; set; } = DateTime.Today;

        [JsonIgnore]
        [XmlIgnore]
        public ICommand Remove { get; set; }

        public Note()
        {
            Remove = new DeleteButtonEvent();
        }

        /// <summary>
        /// Init Note item class
        /// </summary>
        /// <param name="detailNote">Default values</param>
        public Note(Note detailNote)
        {
            ID = detailNote.ID;
            Name = detailNote.Name;
            Description = detailNote.Description;
            Secured = detailNote.Secured;
            Created = detailNote.Created;

            Remove = detailNote.Remove;
        }

        public override object Clone()
        {
            return new Note(this);
        }
    }
}
