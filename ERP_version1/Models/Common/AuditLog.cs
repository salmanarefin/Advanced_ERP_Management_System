using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.Common
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(150)]
        public string? UserEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ModuleName { get; set; }

        [StringLength(100)]
        public string? TableName { get; set; }

        public int? RecordId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}