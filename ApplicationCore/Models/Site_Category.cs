using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Site_Category
    {
        [Key]
        [Display(Name = "ID")]
        public int SiteCategory { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Name")]
        public string SiteCategoryName { get; set; }

        [Display(Name = "Description")]
        public string SiteCategoryDescription { get; set; }

        [Required]
        [Display(Name = "Location ID")]
        public int LocationID { get; set; }

        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }


    }
}
