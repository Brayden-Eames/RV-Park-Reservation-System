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
        public int TypeID { get; set; }

        [Required, MaxLength(50)]
        public string TypeName { get; set; }

        public string TypeDescription { get; set; }
    }
}
