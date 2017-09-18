using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Classification: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A classification name is required")]
        [Display(Name ="Classification")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }

    }
}