using System;

namespace Project.DTOs
{
    public class ProgressDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProgressID { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
