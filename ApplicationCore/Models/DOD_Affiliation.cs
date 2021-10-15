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
        public int DODAffiliationID { get; set; }

        [Required, MaxLength(50)]
        public string DODAffiliationType { get; set; }
    }
}
