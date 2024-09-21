namespace Project.DTO
{
    public class ProgressReportDto
    {
        public string StudentID { get; set; }
        public int TeacherID { get; set; }
        public IFormFile File { get; set; }
    }
}
