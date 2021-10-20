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
        public int EventID { get; set; }

        [Required, MaxLength(50)]
        public string EventName { get; set; }

        [Required]
        public DateTime EventStartDate { get; set; }

        [Required]
        public DateTime EventEndDate { get; set; }

        public string EventDescription { get; set; }

        public int Daily_Surcharge { get; set; }

        public int Weekly_Surcharge { get; set; }
       

        [Required]
        public int LocationID { get; set; }

        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }
       

    }
}
