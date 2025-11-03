using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    [Table("Permissions")]
    public class Permissions
    {
        [Key]
        [Column("permission_id")]
        public int PermissionId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("permission_name")]
        public string PermissionName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        [Column("module")]
        public string Module { get; set; } = string.Empty;

        [Column("create_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("update_at")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
    }
}