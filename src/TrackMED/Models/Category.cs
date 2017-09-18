using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Category: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A category description is required")]
        [Display(Name = "Category")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}