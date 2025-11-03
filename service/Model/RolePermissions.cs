using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    [Table("RolePermissions")]
    public class RolePermissions
    {
        [Key]
        [Column("role_permission_id")]
        public int RolePermissionId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("permission_id")]
        public int PermissionId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]  
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("PermissionId")]  
        public virtual Permissions Permission { get; set; } = null!;
    }
}