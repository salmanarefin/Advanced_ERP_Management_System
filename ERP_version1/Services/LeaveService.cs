using ERP_version1.Data;
using ERP_version1.Models.LeaveModule;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public LeaveService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<List<LeaveApplication>> GetAllLeaveApplicationsAsync()
        {
            return await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .OrderByDescending(l => l.AppliedAt)
                .ToListAsync();
        }

        public async Task<LeaveApplication?> GetLeaveApplicationDetailsAsync(int id)
        {
            return await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(l => l.LeaveApplicationId == id);
        }

        public async Task<LeaveApplication?> GetLeaveApplicationForActionAsync(int id)
        {
            return await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(l => l.LeaveApplicationId == id);
        }

        public async Task<string?> ApplyLeaveAsync(LeaveApplication leaveApplication)
        {
            if (leaveApplication.EmployeeId <= 0)
            {
                return "Please select an employee.";
            }

            if (leaveApplication.LeaveTypeId <= 0)
            {
                return "Please select a leave type.";
            }

            if (leaveApplication.EndDate < leaveApplication.StartDate)
            {
                return "End date cannot be before start date.";
            }

            if (leaveApplication.StartDate.Date < DateTime.Today)
            {
                return "Leave start date cannot be in the past.";
            }

            bool hasOverlap = await _context.LeaveApplications.AnyAsync(l =>
                l.EmployeeId == leaveApplication.EmployeeId &&
                l.Status != "Rejected" &&
                l.StartDate <= leaveApplication.EndDate &&
                l.EndDate >= leaveApplication.StartDate);

            if (hasOverlap)
            {
                return "This employee already has a leave application in this date range.";
            }

            var employee = await _context.Employees.FindAsync(leaveApplication.EmployeeId);

            if (employee == null)
            {
                return "Employee not found.";
            }

            var leaveType = await _context.LeaveTypes.FindAsync(leaveApplication.LeaveTypeId);

            if (leaveType == null)
            {
                return "Leave type not found.";
            }

            leaveApplication.TotalDays = (leaveApplication.EndDate.Date - leaveApplication.StartDate.Date).Days + 1;
            leaveApplication.Status = "Pending";
            leaveApplication.AppliedAt = DateTime.Now;
            leaveApplication.ApprovedAt = null;
            leaveApplication.RejectionReason = null;

            _context.LeaveApplications.Add(leaveApplication);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Create",
                "Leave",
                "LeaveApplications",
                leaveApplication.LeaveApplicationId,
                $"{employee.FullName} applied for {leaveType.LeaveTypeName} from {leaveApplication.StartDate:dd MMM yyyy} to {leaveApplication.EndDate:dd MMM yyyy}."
            );

            return null;
        }

        public async Task<string?> ApproveLeaveAsync(int id)
        {
            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(l => l.LeaveApplicationId == id);

            if (leaveApplication == null)
            {
                return "Leave application not found.";
            }

            if (leaveApplication.Status != "Pending")
            {
                return "Only pending applications can be approved.";
            }

            leaveApplication.Status = "Approved";
            leaveApplication.ApprovedAt = DateTime.Now;
            leaveApplication.RejectionReason = null;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Approve",
                "Leave",
                "LeaveApplications",
                leaveApplication.LeaveApplicationId,
                $"{leaveApplication.Employee?.FullName}'s {leaveApplication.LeaveType?.LeaveTypeName} application approved."
            );

            return null;
        }

        public async Task<string?> RejectLeaveAsync(int id, string rejectionReason)
        {
            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(l => l.LeaveApplicationId == id);

            if (leaveApplication == null)
            {
                return "Leave application not found.";
            }

            if (leaveApplication.Status != "Pending")
            {
                return "Only pending applications can be rejected.";
            }

            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                return "Rejection reason is required.";
            }

            leaveApplication.Status = "Rejected";
            leaveApplication.RejectionReason = rejectionReason;
            leaveApplication.ApprovedAt = null;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Reject",
                "Leave",
                "LeaveApplications",
                leaveApplication.LeaveApplicationId,
                $"{leaveApplication.Employee?.FullName}'s {leaveApplication.LeaveType?.LeaveTypeName} application rejected. Reason: {rejectionReason}"
            );

            return null;
        }

        public async Task<LeaveApplication?> GetLeaveApplicationForDeleteAsync(int id)
        {
            return await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(l => l.LeaveApplicationId == id);
        }

        public async Task<bool> DeleteLeaveApplicationAsync(int id)
        {
            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(l => l.LeaveApplicationId == id);

            if (leaveApplication == null)
            {
                return false;
            }

            string employeeName = leaveApplication.Employee?.FullName ?? "Unknown Employee";
            string leaveType = leaveApplication.LeaveType?.LeaveTypeName ?? "Unknown Leave Type";

            _context.LeaveApplications.Remove(leaveApplication);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Delete",
                "Leave",
                "LeaveApplications",
                id,
                $"Leave application deleted for {employeeName}. Leave type: {leaveType}."
            );

            return true;
        }
    }
}