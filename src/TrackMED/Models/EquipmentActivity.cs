using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class EquipmentActivity: IEntity
    {
        public string Id { get; set; }
        public string DeploymentID { get; set; }
        public string imte { get; set; }
        public string Work_Order { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "WO Scheduled Due")]
        public DateTime? WO_Scheduled_Due { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "WO Done Date")]
        public DateTime? WO_Done_Date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "WO Calculated Due")]
        public DateTime? WO_Calculated_Due_Date { get; set; }

        public string Schedule { get; set; }
        public string eRecord { get; set; }

        public string ActivityTypeID { get; set; }
        public string ProviderOfServiceID { get; set; }
        public string StatusID { get; set; }

        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        public virtual ActivityType ActivityType { get; set; }
        public virtual ProviderOfService ProviderOfService { get; set; }
        public virtual Status Status { get; set; }
    }
}