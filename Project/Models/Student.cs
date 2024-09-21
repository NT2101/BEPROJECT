using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblStudent")]
    public class Student
    {
        [Key]
        [MaxLength(14)]
        public string StudentID { get; set; }

        [ForeignKey("tblAccount")]
        public int AccountID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime Dob { get; set; }

        public int Sex { get; set; }

        [MaxLength(350)]
        public string Address { get; set; }

        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [MaxLength(350)]
        public string EmailAddress { get; set; }

        [MaxLength(350)]
        public string Country { get; set; }

        [MaxLength(300)]
        public int Status { get; set; }

        public int StatusTopic { get; set; }

        public int StatusProgess { get; set; }

        public string ClassID { get; set; }

        [ForeignKey("ClassID")]
        public Class Class { get; set; }

        public DateTime CreatedDate { get; set; }

        [MaxLength(50)]
        public string CreatedUser { get; set; }

        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string ModifiedUser { get; set; }

        public Account account { get; set; }

       
    }
}
