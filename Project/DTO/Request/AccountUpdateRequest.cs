namespace Project.DTO.Request
{
    public class AccountUpdateRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public int Status { get; set; }
    }
}
