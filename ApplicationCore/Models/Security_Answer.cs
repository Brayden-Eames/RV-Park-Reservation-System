using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Security_Answer
    {
        [Key]
        public int Answer { get; set; }

        [Required, MaxLength(50)]
        public string AnswerText { get; set; }

        [Required]
        public string Id { get; set; }

        [ForeignKey("Id")]
        public virtual Customer Customer { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [ForeignKey("QuestionID")]
        public virtual Security_Question Security_Question { get; set; }


    }
}
