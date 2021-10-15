using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Site
    {
        [Key]
        public int SiteID { get; set; }

        [Required, MaxLength(5)]
        public string SiteNumber { get; set; }

        [Required]
        public int SiteLength { get; set; }

        public string SiteDescription { get; set; }

        [MaxLength(10)]
        public string SiteLastModifiedBy { get; set; }

        public DateTime SiteLastModifiedDate { get; set; }

        [Required]
        public int SiteCategoryID { get; set; }

        [ForeignKey("SiteCategoryID")]
        public virtual Site_Category Site_Category { get; set; }
    }

}
