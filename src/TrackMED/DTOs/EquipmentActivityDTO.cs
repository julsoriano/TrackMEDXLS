using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;

namespace TrackMED.DTOs
{
    public class EquipmentActivityDTO
    {
        public string Id { get; set; }
        public string DeploymentID { get; set; }
        public string imte { get; set; }
        public string Work_Order { get; set; }
        public DateTime? WO_Scheduled_Due { get; set; }
        public DateTime? WO_Done_Date { get; set; }
        public DateTime? WO_Calculated_Due_Date { get; set; }
        public string Schedule { get; set; }
        public string eRecord { get; set; }

        public string ActivityTypeID { get; set; }
        public string ServiceProviderID { get; set; }
        public string StatusID { get; set; }

        public virtual ActivityType ActivityType { get; set; }
        public virtual ProviderOfService ServiceProvider { get; set; }
        public virtual Status Status { get; set; }

        public string SystemID { get; set; }
        public DateTime? DeploymentDate { get; set; }
    }
}
