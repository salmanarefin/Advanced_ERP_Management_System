using ERP_version1.Models.HR;
using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.LeaveModule
{
    public class LeaveApplication : IValidatableObject
    {
        public int LeaveApplicationId { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Leave type is required.")]
        public int LeaveTypeId { get; set; }

        public LeaveType? LeaveType { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [Range(0, 365, ErrorMessage = "Total days must be between 0 and 365.")]
        public int TotalDays { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot be more than 500 characters.")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Leave status is required.")]
        public string Status { get; set; } = "Pending";

        [StringLength(500, ErrorMessage = "Rejection reason cannot be more than 500 characters.")]
        public string? RejectionReason { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        public DateTime? ApprovedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.Date < StartDate.Date)
            {
                yield return new ValidationResult(
                    "End date cannot be before start date.",
                    new[] { nameof(EndDate) });
            }

            int calculatedDays = (EndDate.Date - StartDate.Date).Days + 1;

            if (calculatedDays <= 0)
            {
                yield return new ValidationResult(
                    "Total leave days must be greater than zero.",
                    new[] { nameof(TotalDays) });
            }

            if (TotalDays > 0 && TotalDays != calculatedDays)
            {
                yield return new ValidationResult(
                    "Total days does not match the selected start and end date.",
                    new[] { nameof(TotalDays) });
            }

            List<string> validStatuses = new List<string>
            {
                "Pending",
                "Approved",
                "Rejected"
            };

            if (!validStatuses.Contains(Status))
            {
                yield return new ValidationResult(
                    "Leave status must be Pending, Approved, or Rejected.",
                    new[] { nameof(Status) });
            }

            if (Status == "Rejected" && string.IsNullOrWhiteSpace(RejectionReason))
            {
                yield return new ValidationResult(
                    "Rejection reason is required when leave is rejected.",
                    new[] { nameof(RejectionReason) });
            }

            if (Status == "Approved" && ApprovedAt == null)
            {
                ApprovedAt = DateTime.Now;
            }
        }
    }
}