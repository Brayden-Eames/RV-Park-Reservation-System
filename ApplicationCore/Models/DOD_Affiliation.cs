using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class DOD_Affiliation
    {
        [Key]
        [Display(Name = "ID")]
        public int DODAffiliationID { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "DOD Affiliation")]
        public string DODAffiliationType { get; set; }
    }
}
