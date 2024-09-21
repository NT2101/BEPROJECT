using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class AccountCreationRequest
    {
        public string Name { get; set; }

        public string Password { get; set; }
    }
}
