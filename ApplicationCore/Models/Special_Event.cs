using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Special_Event
    {
        [Key]
        [Display(Name = "ID")]
        public int EventID { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Name")]
        public string EventName { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime EventStartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EventEndDate { get; set; }

        [Display(Name = "Description")]
        public string EventDescription { get; set; }

        [Display(Name = "Daily Surcharge")]
        public int Daily_Surcharge { get; set; }

        [Display(Name = "Weekly Surcharge")]
        public int Weekly_Surcharge { get; set; }
       

        [Required]
        [Display(Name = "Location ID")]
        public int LocationID { get; set; }

        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }
       

    }
}
