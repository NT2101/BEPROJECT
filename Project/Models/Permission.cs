using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblPermissions")]//Phân quyền người dùng

    public class Permission
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        [MaxLength(100)]
        public string PermissionName { get; set; }
        public bool CanManageAccounts { get; set; }
        public bool CanManageRoles { get; set; }
        public bool CanManagePermissions { get; set; }
        public bool CanUpdateProgress { get; set; }
        public bool CanRegisterTopics { get; set; }
        public bool CanUpdateAccounts { get; set; }
        public bool CanRequestTopicChanges { get; set; }
        public bool CanConfirmThesis { get; set; }
        public bool CanManageTheses { get; set; }
        public bool CanRequestGuidance { get; set; }
        public bool CanConfirmGuidance { get; set; }
        public bool CanApproveTopicChanges { get; set; }
        public bool CanUpdatePersonalInfo { get; set; }
        public bool CanManageDepartments { get; set; }
        public bool CanManageStudents { get; set; }
        public bool CanManageTeachers { get; set; }
        public bool CanUpdateTopics { get; set; }
        public bool CanAssignGuidance { get; set; }
        public bool CanManageSpecializations { get; set; }
        public bool CanAssignCouncils { get; set; }
        public bool CanManageClasses { get; set; }
        public bool CanManageProgress { get; set; }
        public bool CanManageFields { get; set; }
        public bool CanManageFaculties { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }
    }
}
