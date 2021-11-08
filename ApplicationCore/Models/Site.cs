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
        [Display(Name = "ID")]
        public int SiteID { get; set; }

        [Required, MaxLength(5)]
        [Display(Name = "Site Number")]
        public string SiteNumber { get; set; }

        [Required]
        [Display(Name = "Length")]
        public int SiteLength { get; set; }

        [Display(Name = "Description")]
        public string SiteDescription { get; set; }

        [MaxLength(10)]
        [Display(Name = "Modified By")]
        public string SiteLastModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime SiteLastModifiedDate { get; set; }

        [Required]
        [Display(Name = "Site Category ID")]
        public int SiteCategoryID { get; set; }

        [ForeignKey("SiteCategoryID")]
        public virtual Site_Category Site_Category { get; set; }
    }

}
