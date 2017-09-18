using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Manufacturer: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Manufacturerer name is required")]
        [Display(Name ="Manufacturer")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}