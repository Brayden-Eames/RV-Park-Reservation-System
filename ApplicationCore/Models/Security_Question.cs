using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Security_Question
    {
        [Key]
        public int QuestionID { get; set; }

        [Required, MaxLength(200)]
        public string QuestionText { get; set; }
    }
}
