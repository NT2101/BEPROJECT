namespace Project.DTO
{
    public class GetProgressByTeacherIDDTO
    {
        public int ReportID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public int TeacherID { get; set; }
        public string TeacherName { get; set; }
        public int ProgressID { get; set; }
        public string ProgressTitle { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public string Comments { get; set; }
    }

}
