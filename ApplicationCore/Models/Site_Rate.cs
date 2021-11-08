using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Site_Rate
    {
        [Key]
        [Display(Name = "ID")]
        public int RateID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")]
        public decimal RateAmount { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime RateStartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime RateEndDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Modified By")]
        public string RateLastModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime RateModifiedDate { get; set; }

        [Required]
        [Display(Name = "Site Category ID")]
        public int SiteCategoryID { get; set; }

        [ForeignKey("SiteCategoryID")]
        public virtual Site_Category Site_Category { get; set; }


    }
}
