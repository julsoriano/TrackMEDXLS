using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Status: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A status description is required")]
        [Display(Name ="Status")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}