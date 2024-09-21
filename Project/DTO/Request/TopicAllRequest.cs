namespace Project.DTO.Request
{
    public class TopicAllRequest
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string StudentID { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; } // Assuming Class has a ClassName property
        public string TeacherName { get; set; }
    }
}
