using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Service_Status_Type
    {
        [Key]
        [Display(Name = "ID")]
        public int ServiceStatusID { get; set; }

        [Required, MaxLength(20)]
        [Display(Name = "Service Status")]
        public string ServiceStatusType { get; set; }
    }
}
