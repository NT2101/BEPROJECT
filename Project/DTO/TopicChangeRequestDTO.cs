namespace Project.DTO
{
    public class TopicChangeRequestDTO
    {
        public string StudentID { get; set; }
        public int TopicID { get; set; }
        public int TeacherID { get; set; }
        public int FieldID { get; set; }
        public string NewTitle { get; set; }
        public string NewDescription { get; set; }
        public string ReasonForChange { get; set; }
    }
}
