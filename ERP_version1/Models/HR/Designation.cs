using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_version1.Models.HR
{
    public class Designation
    {
        public int DesignationId { get; set; }

        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}