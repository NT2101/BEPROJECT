using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblTeacher")]
    public class Teacher
    {
        [Key]
        public int TeacherID { get; set; }

        [ForeignKey("tblAccount")]
        public int AccountID { get; set; }

        [ForeignKey("tblFaculties")]
        public string FacultyID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime Dob { get; set; }

        // Allowing null values
        public int? Sex { get; set; }  // Nullable integer for gender

        [MaxLength(11)]
        public string PhoneNumber { get; set; }  // Nullable string

        [MaxLength(350)]
        public string EmailAddress { get; set; }

        [MaxLength]
        public string Description { get; set; }  // Nullable string

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }

        [MaxLength(50)]
        public string CreatedUser { get; set; }

        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string ModifiedUser { get; set; }

        public Account account { get; set; }

        public Faculty faculty { get; set; }

    }
}
