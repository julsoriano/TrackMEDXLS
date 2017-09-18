using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Location: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A location name is required")]
        [Display(Name ="Location")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}