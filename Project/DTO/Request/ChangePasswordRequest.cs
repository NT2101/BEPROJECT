using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class ChangePasswordRequest
    {
        [Required]
        public int AccountID { get; set; }

        [Required]
        [MaxLength(50)]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(50)]
        public string NewPassword { get; set; }
    }
}
