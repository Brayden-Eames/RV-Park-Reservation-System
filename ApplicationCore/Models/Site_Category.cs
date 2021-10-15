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
        public int SiteCategory { get; set; }

        [Required, MaxLength(50)]
        public string SiteCategoryName { get; set; }

        public string SiteCategoryDescription { get; set; }

        [Required]
        public int LocationID { get; set; }

        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }


    }
}
