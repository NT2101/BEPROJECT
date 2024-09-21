using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblTopics")]//Cập nhật Đề tài

    public class Topic
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(400)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        public string CreatedUser { get; set; }

        public int status {  get; set; }
        public String? StudentID { get; set; }
        public int TeacherID { get; set; }

        public int FieldID { get; set; }
        public int RegistrationDate { get; set; }

        [ForeignKey("StudentID")]
        public Student Student { get; set; }
        [ForeignKey("TeacherID")]
        public Teacher Teacher { get; set; }

        [ForeignKey("FieldID")]
        public Field Field { get; set; }
    }

}
