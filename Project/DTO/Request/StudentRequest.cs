using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class StudentRequest
    {
        public string StudentID { get; set; }
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

        public string classID {  get; set; }
        public DateTime ModifiedDate {  get; set; }
        public string ModifiedUser {  get; set; }

    }
}
