namespace Project.DTO
{
    public class TeacherDTO
    {
        public int TeacherID { get; set; }
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public int? Sex { get; set; }  // Nullable integer for gender
        public string? PhoneNumber { get; set; }  // Nullable string
        public string EmailAddress { get; set; }
        public string? Description { get; set; }  // Nullable string
        public string FacultyID { get; set; }
        public int Status { get; set; }
    }
}
