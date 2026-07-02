using ERP_version1.Data;
using ERP_version1.Models.AttendanceModule;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public AttendanceService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<List<Attendance>> GetAllAttendancesAsync()
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AttendanceDate)
                .ThenByDescending(a => a.CheckInTime)
                .ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceDetailsAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<Attendance?> GetAttendanceForDeleteAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<string?> CheckInAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                return "Please select an employee.";
            }

            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return "Employee not found.";
            }

            DateTime today = DateTime.Today;

            bool alreadyCheckedIn = await _context.Attendances
                .AnyAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == today);

            if (alreadyCheckedIn)
            {
                return "This employee has already checked in today.";
            }

            DateTime now = DateTime.Now;

            TimeSpan officeStartTime = new TimeSpan(9, 0, 0);
            TimeSpan currentTime = now.TimeOfDay;

            string status = "Present";
            int lateMinutes = 0;

            if (currentTime > officeStartTime)
            {
                status = "Late";
                lateMinutes = (int)(currentTime - officeStartTime).TotalMinutes;
            }

            Attendance attendance = new Attendance
            {
                EmployeeId = employeeId,
                AttendanceDate = today,
                CheckInTime = now,
                Status = status,
                LateMinutes = lateMinutes,
                WorkingHours = 0,
                CreatedAt = now
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Check In",
                "Attendance",
                "Attendances",
                attendance.AttendanceId,
                $"{employee.FullName} checked in at {now:hh:mm tt}. Status: {status}."
            );

            return null;
        }

        public async Task<string?> CheckOutAsync(int attendanceId)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);

            if (attendance == null)
            {
                return "Attendance record not found.";
            }

            if (attendance.CheckInTime == null)
            {
                return "Check-in time not found.";
            }

            if (attendance.CheckOutTime != null)
            {
                return "This employee has already checked out.";
            }

            DateTime now = DateTime.Now;

            attendance.CheckOutTime = now;
            attendance.WorkingHours = Math.Round((now - attendance.CheckInTime.Value).TotalHours, 2);

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Check Out",
                "Attendance",
                "Attendances",
                attendance.AttendanceId,
                $"{attendance.Employee?.FullName} checked out at {now:hh:mm tt}. Working hours: {attendance.WorkingHours}."
            );

            return null;
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
            {
                return false;
            }

            string employeeName = attendance.Employee?.FullName ?? "Unknown Employee";

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Delete",
                "Attendance",
                "Attendances",
                id,
                $"Attendance record deleted for {employeeName}."
            );

            return true;
        }
    }
}