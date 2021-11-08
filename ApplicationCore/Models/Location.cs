using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Location
    {
        [Key]
        [Display(Name = "ID")]
        public int LocationID { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Address Line 1")]
        public string LocationAddress1 { get; set; }

        [MaxLength(150)]
        [Display(Name = "Address Line 2")]
        public string LocationAddress2 { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "City")]
        public string LocationCity { get; set; }

        [Required, MaxLength(20)]
        [Display(Name = "State")]
        public string LocationState { get; set; }

        [Required, MaxLength(10)]
        [Display(Name = "Zip")]
        public string LocationZip { get; set; }



    }

}
