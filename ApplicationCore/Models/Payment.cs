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
        [Display(Name = "ID")]
        public int PayID { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime PayDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Cost")]
        public decimal PayTotalCost { get; set; }

        [Required]
        [Display(Name = "Is Paid")]
        public Boolean IsPaid { get; set; }

        [MaxLength(40)]
        [Display(Name = "CC Reference")]
        public string CCReference { get; set; }

        [MaxLength(30)]
        [Display(Name = "Modified By")]
        public string PayLastModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime PayLastModifiedDate { get; set; }

        [Required]
        [Display(Name = "Reservation ID")]
        public int ResID { get; set; }

        [ForeignKey("ResID")]
        public virtual Reservation Reservation { get; set; }

        [Required]
        [Display(Name = "Payment Reason ID")]
        public int PayReasonID { get; set; }

        [ForeignKey("PayReasonID")]
        public virtual Payment_Reason Payment_Reason { get; set; }

        [Display(Name = "Payment Type ID")]
        public int PayTypeID { get; set; }

        [ForeignKey("PayTypeID")]
        public virtual Payment_Type Payment_Type { get; set; }
    }
}

