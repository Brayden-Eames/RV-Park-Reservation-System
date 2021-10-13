using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;

namespace ApplicationCore.Models
{
    public class Customer : IdentityUser
    {
        //add in all columns from schema

        [Required]
        [Display(Name = "First Name")]
        public string CustFirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string CustLastName { get; set; }

        [Required]
        [Display(Name ="Email Address")] //might potentially need to be a composite key
        public string CustEmail { get; set; }

        [Required]
        [Display(Name ="Phone Number")]
        public string CustPhone { get; set; }

        public string CustLastModifiedBy { get; set; }

        public DateTime CustLastModifiedDate { get; set; }

        [NotMapped]
        [ForeignKey("ServiceStatusID")]
        public int ServiceStatusId { get; set; }

        [NotMapped]
        [ForeignKey("DODAffiliationID")]
        public int DODAffiliationId { get; set; }
    }
}
