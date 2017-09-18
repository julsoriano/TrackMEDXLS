using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class SystemTab: Equipment
    {
        [Required(ErrorMessage = "A system description is required")]
        public string SystemsDescriptionID { get; set; }
        public string LocationID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "Deployment Date")]
        public DateTime? DeploymentDate { get; set; }

        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }

        public virtual SystemsDescription SystemsDescription { get; set; }
        public virtual Location Location { get; set; }

        public List<string> leftComponents { get; set; }
        public List<string> rightComponents { get; set; }
    }
}