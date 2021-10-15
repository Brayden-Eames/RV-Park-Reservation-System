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
        public int RateID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [DataType(DataType.Currency)]
        public decimal RateAmount { get; set; }

        [Required]
        public DateTime RateStartDate { get; set; }

        [Required]
        public DateTime RateEndDate { get; set; }

        [MaxLength(10)]
        public string RateLastModifiedBy { get; set; }

        
        public DateTime RateModifiedDate { get; set; }

        [Required]
        public int SiteCategoryID { get; set; }

        [ForeignKey("SiteCategoryID")]
        public virtual Site_Category Site_Category { get; set; }


    }
}
