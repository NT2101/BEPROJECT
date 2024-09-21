namespace Project.DTO.Request
{
    public class CommitteeRequest
    {
        public int CommitteeID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
