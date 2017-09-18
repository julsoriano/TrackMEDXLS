using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Description: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A description of the equipment is required")]
        [Display(Name ="Description")]
        public string Desc { get; set; }

        public string Tag { get; set; }

        //[Timestamp]
        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }

        /*
        [Timestamp]
        public byte[] RowVersion { get; set; }
        */
    }
}
