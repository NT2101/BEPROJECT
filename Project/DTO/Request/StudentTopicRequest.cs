namespace Project.DTO.Request
{
    public class StudentTopicRequest
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TeacherID { get; set; }
        public int Status { get; set; }

        public string TeacherName { get; set; }
        public int FieldID { get; set; }
        public string FieldName { get; set; }
    }
}
