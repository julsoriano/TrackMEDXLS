using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public abstract class Equipment: IEntity
    {
        public string Id { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "An equipment code is required")]
        [Display(Name="IMTE")]
        public string imte { get; set; }

        [Display(Name ="Serial #")]
        public string serialnumber { get; set; }

        [StringLength(50)]
        public string Notes { get; set; }

        /*
        [Timestamp]
        public byte[] RowVersion { get; set; }
        */

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
        
        [DisplayName("Owner")]
        public string OwnerID { get; set; }

        [DisplayName("Status")]
        public string StatusID { get; set; }

        [DisplayName("Activity")]
        public string ActivityTypeID { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual Status Status { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        //public virtual ICollection<Event> Events { get; set; }

    }
}