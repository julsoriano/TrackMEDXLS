using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Model: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A model description is required")]
        [Display(Name ="Model")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}