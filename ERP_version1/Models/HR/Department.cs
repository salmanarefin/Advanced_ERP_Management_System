using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.HR
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}