namespace Project.DTO
{
    public class GetProgessByProgessIDDTO
    {
        public string StudentID { get; set; }
        public int TeacherID { get; set; }
        public int ProgressID { get; set; }

        public int ReportID { get; set; }
        public string FileName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string TeacherName { get; set; }

        public string StudentName { get; set; }
        public string ProgessName { get; set; }
    }
}
