namespace Project.DTO.Request
{
    public class FullTopicRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string StudentID { get; set; }
        public int TeacherID { get; set; }
        public int FieldID { get; set; }
    }
}
