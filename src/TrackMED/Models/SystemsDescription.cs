using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class SystemsDescription: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A description of the system is required")]
        [Display(Name ="Systems Description")]
        public string Desc { get; set; }

        //[Timestamp]
        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }

        /*
        [Timestamp]
        public byte[] RowVersion { get; set; }
        */
    }
}
