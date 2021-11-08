using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Vehicle_Type
    {
        [Key]
        [Display(Name = "ID")]
        public int TypeID { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Name")]
        public string TypeName { get; set; }

        [Display(Name = "Description")]
        public string TypeDescription { get; set; }
    }
}
