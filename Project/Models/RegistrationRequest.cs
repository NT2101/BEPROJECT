using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{

    [Table("tblRegistrationRequest")]//Quản lý tài khoản
    public class RegistrationRequest
    {
        [Key]
        public int RequestId { get; set; }
        [ForeignKey("tblStudent")]
        public string StudentId { get; set; }
        [ForeignKey("tblTeacher")]
        public int TeacherId { get; set; }
        public bool IsConfirmed { get; set; }

        public Student Student;

        public   Teacher Teacher;

    }
}
