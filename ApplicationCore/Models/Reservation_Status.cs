using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Reservation_Status
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        public int ResStatusID { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Name")]
        public string ResStatusName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Description")]
        public string ResStatusDescription { get; set; }
    }

}
