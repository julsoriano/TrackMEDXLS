using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class ActivityType: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Activity Type is required")]
        [Display(Name ="Activity")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}