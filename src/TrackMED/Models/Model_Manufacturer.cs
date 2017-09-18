using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Model_Manufacturer: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A model/manufacturer description is required")]
        [Display(Name ="Model/Manufacturer")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}