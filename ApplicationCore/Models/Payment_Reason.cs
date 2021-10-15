using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Payment_Reason
    {
        [Key]
        public int PayReasonID { get; set; }

        [Required, MaxLength(20)]
        public string PayReasonName { get; set; }
    }

}
