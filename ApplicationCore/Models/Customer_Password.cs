using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Customer_Password
    {
        [Key]
        public int CustPassID { get; set; }

        [Required, MaxLength(40)]
        public string Password { get; set; }

        [Required]
        public Boolean Active { get; set; }

        [Required]
        public DateTime PasswordAssignedDate { get; set; }

        [Required]
        public string Id { get; set; }

        [ForeignKey("Id")]
        public virtual Customer Customer { get; set; }

    }
}
