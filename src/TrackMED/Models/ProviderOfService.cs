using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class ProviderOfService: IEntity
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Service Provider name is required")]
        [Display(Name ="Service Provider")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}