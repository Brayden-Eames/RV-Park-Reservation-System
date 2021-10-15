using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Payment_Type
    {
        [Key]
        public int PayTypeID { get; set; }

        [Required, MaxLength(11)]
        public string PayType { get; set; }
    }
}
