using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Location
    {
        [Key]
        public int LocationID { get; set; }

        [Required, MaxLength(100)]
        public string LocationName { get; set; }

        [Required, MaxLength(150)]
        public string LocationAddress1 { get; set; }

        [MaxLength(150)]
        public string LocationAddress2 { get; set; }

        [Required, MaxLength(50)]
        public string LocationCity { get; set; }

        [Required, MaxLength(20)]
        public string LocationState { get; set; }

        [Required, MaxLength(10)]
        public string LocationZip { get; set; }



    }

}
