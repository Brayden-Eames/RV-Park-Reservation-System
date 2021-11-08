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
        public int ResID { get; set; }

        [Required]
        public int ResNumAdults { get; set; }

        [Required]
        public int ResNumChildren { get; set; } 

        [Required]
        public int ResNumPets { get; set; } 

        public Boolean ResAcknowledgeValidPets { get; set; }
        
        [Required]
        public DateTime ResStartDate { get; set; } 

        [Required]
        public DateTime ResEndDate { get; set; }   //might potentially need to be a Composite key

        [Required]
        public DateTime ResCreatedDate { get; set; }   //might potentially need to be a Domain key

        public string ResComment { get; set; }

        public int ResVehicleLength { get; set; }

        [Required, MaxLength(40)]
        public string ResLastModifiedBy { get; set; }

        public DateTime ResLastModifiedDate { get; set; }

        public int TypeID { get; set; }

        [ForeignKey("TypeID")]
        [Required]
        public virtual Vehicle_Type Vehicle_Type { get; set; }

        public string Id { get; set; }

        
        [ForeignKey("Id")]
        [Required]
        public virtual Customer Customer { get; set; }

        public int SiteID { get; set; }
        [ForeignKey("SiteID")]
        public virtual Site Site { get; set; }


        public int ResStatusID { get; set; }

        [ForeignKey("ResStatusID")]
        [Required]
        public virtual Reservation_Status Reservation_Status { get; set; }
    }


}
