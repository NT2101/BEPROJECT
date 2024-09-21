namespace Project.DTO.Request
{
    public class TopicRequestByTeacher
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string StudentID { get; set; }
        public string ClassID { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; }
    }
}
