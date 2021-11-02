using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Payment
    {
        [Key]
        public int PayID { get; set; }

        [Required]
        public DateTime PayDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [DataType(DataType.Currency)]
        public decimal PayTotalCost { get; set; }

        [Required]
        public Boolean IsPaid { get; set; }

        [MaxLength(40)]
        public string CCReference { get; set; }

        [MaxLength(30)]
        public string PayLastModifiedBy { get; set; }

        public DateTime PayLastModifiedDate { get; set; }

        [Required]
        public int ResID { get; set; }

        [ForeignKey("ResID")]
        public virtual Reservation Reservation { get; set; }

        [Required]
        public int PayReasonID { get; set; }

        [ForeignKey("PayReasonID")]
        public virtual Payment_Reason Payment_Reason { get; set; }

        
        public int PayTypeID { get; set; }

        [ForeignKey("PayTypeID")]
        public virtual Payment_Type Payment_Type { get; set; }
    }
}

