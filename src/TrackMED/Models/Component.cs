using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMEDXLS.Models
{
    public class Component: Equipment
    {
        /*
        [Display(Name = "Asset #")]
        public string assetnumber { get; set; }

        [Required(ErrorMessage = "An equipment description is required")]
        [DisplayName("Component Id")]
        public string DescriptionID { get; set; }
        public string Model_ManufacturerID { get; set; }
        public string ServiceProviderID { get; set; }

        public virtual Description Description { get; set; }
        public virtual Model_Manufacturer Model_Manufacturer { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        */

        [Display(Name = "Asset #")]
        [JsonProperty(PropertyName = "assetNumber")]
        public string assetnumber { get; set; }

        [Required(ErrorMessage = "An equipment description is required")]
        [DisplayName("Component Id")]
        [JsonProperty(PropertyName = "descriptionID")]
        public string DescriptionID { get; set; }

        [JsonProperty(PropertyName = "model_ManufacturerID")]
        public string Model_ManufacturerID { get; set; }

        [JsonProperty(PropertyName = "serviceProviderID")]
        public string ProviderOfServiceID { get; set; }

        [JsonProperty(PropertyName = "description")]
        public virtual Description Description { get; set; }

        [JsonProperty(PropertyName = "model_Manufacturer")]
        public virtual Model_Manufacturer Model_Manufacturer { get; set; }

        [JsonProperty(PropertyName = "providerOfService")]
        public virtual ProviderOfService ProviderOfService { get; set; }
        /*
         * You can use the DisplayFormat attribute by itself, but it's generally a good idea to use the DataType attribute also. 
         * The DataType attribute conveys the semantics of the data as opposed to how to render it on a screen, and provides the following benefits that you don't get with DisplayFormat:
           1. The browser can enable HTML5 features (for example to show a calendar control, the locale-appropriate currency symbol, email links, some client-side input validation, etc.).
           2. By default, the browser will render data using the correct format based on your locale.
           3. The DataType attribute can enable MVC to choose the right field template to render the data (the DisplayFormat uses the string template). 
         * For more information, see Brad Wilson's ASP.NET MVC 2 Templates. (Though written for MVC 2, this article still applies to the current version of ASP.NET MVC.)
         * 
         * Also:
         * If you use the DataType attribute with a date field, you have to specify the DisplayFormat attribute also in order to ensure that the field renders correctly in Chrome browsers. 
         * For more information, see this StackOverflow thread. (http://stackoverflow.com/questions/12633471/mvc4-datatype-date-editorfor-wont-display-date-value-in-chrome-fine-in-interne)
         */

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "Calibration Due Date")]
        public DateTime? CalibrationDate { get; set; }
        [Display(Name = "Calibration Interval (Days)")]
        public int? CalibrationInterval { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "Maintenance Due Date")]
        public DateTime? MaintenanceDate { get; set; }
        [Display(Name = "Maintenance Interval (Days)")]
        public int? MaintenanceInterval { get; set; }

        [Display(Name = "Module")]
        public string imteModule { get; set; }

        //[Display(Name = "Desc+Tag")]
        /*
        public string DescTag
        {
            get
            {
                if (Description.Desc != null)
                    return Description.Desc + (!String.IsNullOrEmpty(Description.Tag) ? " Tag: " + Description.Tag : null);
                else return null;
                //return !String.IsNullOrEmpty(Description.Desc) ? Description.Desc + (!String.IsNullOrEmpty(Description.Tag) ? " Tag: " + Description.Tag: null) : null;
            }
        }
        */
        /*
        [Display(Name = "IMTE+")]
        public string IMTECalDate
        {
            get
            {
                return imte + " " + (CalibrationDateTime != null? Convert.ToString(CalibrationDateTime) : null);
            }
        }
        */
    }
}