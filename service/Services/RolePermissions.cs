using CaffePOS.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Services
{

        [Table("RolePermissions")]
        public class RolePermissions
        {
            [Column("role_id")]
            public int RoleId { get; set; }
            [Column("permission_id")]
            public int PermissionId { get; set; }
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Permissions Permission { get; set; } = null!;
    }
}
