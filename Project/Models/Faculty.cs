using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblFaculties")]//Quản lý khoa

    public class Faculty
    {
        [Key]
        public string ID { get; set; }
        [MaxLength(100)]
        public string FacultyName { get; set; }
        public string Description { get; set; }

        public ICollection<Specialization> Specializations { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        public string CreatedUser { get; set; }
        public DateTime ModifiedDate { get; set; }
        [MaxLength(50)]
        public string ModifiedUser { get; set; }
    }
}
