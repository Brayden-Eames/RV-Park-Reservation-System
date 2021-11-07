using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Reservation
    {
        //columns from schema

        [Key]
        [Display(Name = "ID")]
        public int ResID { get; set; }

        [Required]
        [Display(Name = "Number of Adults")]
        public int ResNumAdults { get; set; }

        [Required]
        [Display(Name = "Number of Children")]
        public int ResNumChildren { get; set; } 

        [Required]
        [Display(Name = "Number of Pets")]
        public int ResNumPets { get; set; }

        [Display(Name = "Pet Acknowledgement")]
        public Boolean ResAcknowledgeValidPets { get; set; }
        
        [Required]
        [Display(Name = "Start Date")]
        public DateTime ResStartDate { get; set; } 

        [Required]
        [Display(Name = "End Date")]
        public DateTime ResEndDate { get; set; }   //might potentially need to be a Composite key

        [Required]
        [Display(Name = "Created Date")]
        public DateTime ResCreatedDate { get; set; }   //might potentially need to be a Domain key

        [Display(Name = "Comment")]
        public string ResComment { get; set; }

        [Display(Name = "Vehicle Length")]
        public int ResVehicleLength { get; set; }

        [Required, MaxLength(40)]
        [Display(Name = "Modified By")]
        public string ResLastModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ResLastModifiedDate { get; set; }

        [Display(Name = "Vehicle Type ID")]
        public int VehicleTypeID { get; set; }

        [ForeignKey("TypeID")]
        [Required]
        public virtual Vehicle_Type Vehicle_Type { get; set; }

        [Display(Name = "Customer ID")]
        public string Id { get; set; }

        
        [ForeignKey("Id")]
        [Required]
        public virtual Customer Customer { get; set; }

        [Display(Name = "Site ID")]
        public int SiteID { get; set; }

        [ForeignKey("SiteID")]
        public virtual Site Site { get; set; }

        [Display(Name = "Reservation Status ID")]
        public int ResStatusID { get; set; }

        [ForeignKey("ResStatusID")]
        [Required]
        public virtual Reservation_Status Reservation_Status { get; set; }
    }


}
