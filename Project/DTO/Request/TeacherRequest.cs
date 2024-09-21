using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class TeacherRequest
    {

        public string FacultyID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public int Sex { get; set; }
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        [MaxLength(350)]
        public string EmailAddress { get; set; }
        [MaxLength]
        public string Description { get; set; }
    }
}
