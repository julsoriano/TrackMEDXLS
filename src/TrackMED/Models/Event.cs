using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Event: IEntity
    {
        public string Id { get; set; }

        public string imte { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        //[DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:ddMMMyy hh:mm}")]
        [Display(Name = "Event Date/Time")]
        public DateTime EventDateTime { get; set; }
        public string EquipmentID { get; set; }
        public string StatusID { get; set; }
        public string CategoryID { get; set; }
        public string Validity { get; set; }
        public string LocationID { get; set; }

        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        public virtual Equipment Equipment { get; set; }
        public virtual Status Status { get; set; }
        public virtual Category Category { get; set; }
        public virtual Location Location { get; set; }
    }
}