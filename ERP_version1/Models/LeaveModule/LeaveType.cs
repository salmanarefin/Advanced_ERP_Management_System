using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.LeaveModule
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Leave type name is required.")]
        [StringLength(100, ErrorMessage = "Leave type name cannot be more than 100 characters.")]
        public string LeaveTypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Default days is required.")]
        [Range(0, 365, ErrorMessage = "Default days must be between 0 and 365.")]
        public int DefaultDays { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}