using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Owner: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Owner name is required")]
        [Display(Name ="Owner")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}