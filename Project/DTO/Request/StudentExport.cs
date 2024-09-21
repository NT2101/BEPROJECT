namespace Project.DTO.Request
{
    public class StudentExport
    {
        public string StudentID { get; set; }
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Sex { get; set; } // Change this from int to string
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Country { get; set; }
        public string ClassID { get; set; }
    }
}
