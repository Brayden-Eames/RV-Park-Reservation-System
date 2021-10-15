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
        public int ResStatusID { get; set; }

        [Required, MaxLength(50)]
        public string ResStatusName { get; set; }

        [MaxLength(100)]
        public string ResStatusDescription { get; set; }
    }

}
